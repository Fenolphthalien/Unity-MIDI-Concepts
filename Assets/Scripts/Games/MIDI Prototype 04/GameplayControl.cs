using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityMIDI;

namespace PrototypeFour
{
    [System.Flags]
    public enum EPlayState { AWAITING_START = 0x00, AWAITING_PLAY, PLAY, PAUSE, FAIL, SUCCESS };

    public delegate void Callback();
    public delegate void CallbackWithSender(object sender);
    public delegate void TeleportCallback(Vector3 destination, Vector3 delta);

    public sealed class GameplayControl : BaseGameStateControl, INotePelletContainer, IPlayerShotContainer
    {
        //===============================================
        // Private members.
        //===============================================
        [SerializeField]
        MIDI m_debugMidi = null; //Midi file used when debugging the game.

        [SerializeField]
        Mesh m_debugMesh = null; //Mesh used to show the positions of Note pellets without spawning the actual pellets.

        [SerializeField]
        GameObject m_notePelletPrefab = null, m_polyPelletPrefab = null, m_playerShotPrefab = null; // Prefab reference to the short note pellet.

        [SerializeField]
        Material m_debugMat = null, m_lineMat = null, m_longPolyNoteMat = null; // Material used by m_debugMesh and the material used when rendering the bar lines.

        [SerializeField]
        int m_debugTrack; //Which track to use from m_debugMidi.

        [SerializeField]
        GameObject[] m_dependency = null; // An array of in scene GameObjects that contain scripts that implement IInjectableDependancy.

        [SerializeField]
        Player m_player = null; //reference to the player.

        [SerializeField]
        KillZone m_killZone = null; //Reference to the kill zone.

        [SerializeField]
        AudioSource m_musicPlayer = null; //Reference to an audio source component that handles music playback.

        [SerializeField]
        CameraControl m_cameraControl = null; //Reference to the camera control.

        List<IHorizontalMovementHandler> horizontalMovementHandler; //List of all IHandleHorizontalMovement implementing scripts.

        List<IMoveToDestination> moveToDestinationHandler;//List of all IMoveToDestination implementing scripts.

        List<NotePellet> m_notePellets; //List of all the note pellets alive in the scene.

        List<PlayerShot> m_playerShots;

        float m_gameTime = 0, m_goalTime = 0, m_secondsPerBar = 0; //The time that has elapsed in game. Time until the win condition is called. Cache of the number of seconds per bar.

        float m_speed; //Units per second.

        uint noteLanes
        {
            get
            {
                if (m_procMidi == null)
                    return kMinNoteLanes;
                return kMinNoteLanes < m_procMidi.noteRange ? m_procMidi.noteRange : kMinNoteLanes;
            }
        }  //Number of notelanes.

        float barHeight
        {
            get
            {
                return noteLanes * kLaneHeight;
            }
        } //The current bar height.

        EPlayState m_playState; //The current state.

        ProcessedMIDI m_procMidi; //The current processed MIDI.

        //===============================================
        // public members.
        //===============================================
        public bool parentBars;

        //===============================================
        // Constants
        //===============================================
        const uint kMinNoteLanes = 12, kHalfMinNoteLanes = kMinNoteLanes / 2;

        const int kBarWidth = 40, kCameraXFromPlayer = 10;

        const float kLongPelletMin = 0.5f, kLaneHeight = 20f / kMinNoteLanes, kMinBarHeight = kLaneHeight * kMinNoteLanes;

        //===============================================
        // Base methods.
        //===============================================
        public override void Initialise(MainGameControl mainGameControl)
        {
            base.Initialise(mainGameControl);
            EnableDependencies(false);
            if (m_dependency == null || m_dependency.Length <= 0)
                return;
            for (int i = 0; i < m_dependency.Length; i++)
            {
                MonoBehaviour[] monos = m_dependency[i].GetComponents<MonoBehaviour>();
                horizontalMovementHandler = CastToDependecy<IHorizontalMovementHandler>(monos, horizontalMovementHandler);
                moveToDestinationHandler = CastToDependecy<IMoveToDestination>(monos, moveToDestinationHandler);
            }
        }

        public override void EnterState(StateChangeEventArg arg)
        {
            base.EnterState(arg);
            NewGameArgs newGameArgs = arg as NewGameArgs;
            if (newGameArgs == null)
                return;

            IMIDIProcessor midiProcessor = new PreGame.MIDIProcessor();

            m_procMidi = midiProcessor.ProcessMIDI(newGameArgs.midi, newGameArgs.track);
            if (m_procMidi == null)
            {
                Debug.LogWarning("Processed MIDI is null");
                return;
            }
            m_procMidi.NormaliseSpawnData();

            m_speed = kBarWidth * m_procMidi.bars / m_procMidi.seconds;
            m_secondsPerBar = (float)m_procMidi.seconds / m_procMidi.bars;

            if (m_musicPlayer != null)
            {
                m_musicPlayer.clip = newGameArgs.midi.audioClip;
            }
            if (m_killZone != null)
            {
                m_killZone.Initialise(OnFailureConditionMet, new Vector3(-kBarWidth * 4, 0));
                m_killZone.SetMaxSpeed(2.5f); //Kill zone should always cap at 250% the speed of the player.
                m_killZone.SetHeight(barHeight);
            }

            if (m_player != null)
            {
                m_player.Initialise(new Vector3(-kBarWidth, 0));
                m_player.OnTeleport += OnPlayerTeleport;
            }

            if (m_cameraControl != null)
            {
                m_cameraControl.Initialise(new Vector3(-kBarWidth + kCameraXFromPlayer, 0, -22));
            }

            m_gameTime = -m_secondsPerBar;
            m_goalTime = m_procMidi.seconds;

            m_notePellets = new List<NotePellet>();
            m_playerShots = new List<PlayerShot>();
            for (int i = 0; i < m_procMidi.spawnInfo.Length; i++)
                SpawnNotePellet(m_procMidi.spawnInfo[i]);

            ChangeState(EPlayState.AWAITING_PLAY);
            EnableDependencies(true);
        }

        public override void ExitState()
        {
            ChangeState(EPlayState.AWAITING_START);
            EnableDependencies(false);
            if (m_notePellets != null)
                ClearPelletList();
            if (m_playerShots != null)
                ClearPlayerShotList();
            m_gameTime = 0;
            m_speed = 0;
        }

        //===============================================
        // INoteContainer implementation.
        //===============================================
        public GameObject RemoveNotePellet(NotePellet pellet)
        {
            if (m_notePellets.Remove(pellet))
                return pellet.gameObject; // return the pellets gameObject if stored in the list.
            return null;
        }

        //===============================================
        // IPlayerShotContainer implementation.
        //===============================================
        public PlayerShot Remove(PlayerShot dependancy)
        {
            if (horizontalMovementHandler.Remove(dependancy))
                return dependancy;
            return null;
        }

        //===============================================
        // Mono methods.
        //===============================================
        void Update()
        {
            if (m_playState != EPlayState.PLAY && m_playState != EPlayState.AWAITING_PLAY)
                return;

            if (m_gameTime > m_goalTime)
            {
                OnWinConditionMet();
                return;
            }
            else if (m_playState == EPlayState.AWAITING_PLAY && m_gameTime >= 0)
            {
                ChangeState(EPlayState.PLAY);
                EnableDependencies(true);
                m_killZone.StartMoving();
            }

            Vector3 mousePosition = GetMousePosition();
            float cameraConstraint = (barHeight - kMinBarHeight) * 0.5f;
            cameraConstraint = cameraConstraint > 0 ? cameraConstraint : 0;

            Constraint constraintPlayer = new Constraint(-(barHeight) * 0.5f, (barHeight) * 0.5f);
            Constraint constraintCamera = new Constraint(-cameraConstraint, cameraConstraint);

            if (m_cameraControl != null)
            {
                float cameraBlend = DistanceBetweenKillzoneAndPlayer() / (kBarWidth * 3) + 1;
                m_cameraControl.SetTransformBlend(cameraBlend);
            }

            //Handle LMB down.
            if (m_playState == EPlayState.PLAY && Input.GetMouseButtonDown(0))
            {
                int playerShots = m_player.PeekAtShots();
                if (playerShots > 0)
                {
                    m_player.ReleaseShots();
                    OnPlayerFireShots(playerShots);
                }
            }

            float delta = Time.deltaTime, xdelta = delta * m_speed;
            for (int i = 0; horizontalMovementHandler.Count > i; i++)
            {
                if (horizontalMovementHandler[i] == null)
                {
                    horizontalMovementHandler.RemoveAt(i);
                    continue;
                }
                horizontalMovementHandler[i].HandleHorizontalMovement(xdelta);
            }
            for (int i = 0; moveToDestinationHandler.Count > i; i++)
            {
                if (moveToDestinationHandler[i].Equals(m_cameraControl))
                {
                    moveToDestinationHandler[i].MoveTo(mousePosition, delta, constraintCamera);
                    continue;
                }
                else if (moveToDestinationHandler[i].Equals(m_player))
                {
                    Vector3 target = mousePosition;
                    if (m_notePellets != null && m_notePellets.Count > 0)
                    {
                        //int iterator = 0;
                        //while (iterator < m_notePellets.Count)
                        //{
                        //    if (m_notePellets[i].InSnapBounds(m_player.transform.position, ref target))
                        //        break;
                        //    iterator++;
                        //}
                    }
                    moveToDestinationHandler[i].MoveTo(target, delta, constraintPlayer);
                    continue;
                }
                moveToDestinationHandler[i].MoveTo(mousePosition, delta, constraintPlayer);
            }
            m_gameTime += Time.deltaTime;
        }

        void OnRenderObject()
        {
            if (m_playState != EPlayState.AWAITING_START)
                DrawBars();
        }

        //===============================================
        // Private Methods
        //===============================================
        void DrawBars()
        {
            if (m_procMidi == null || m_lineMat == null)
                return;
            Vector3 start, end, subStart, subEnd;
            float subBarWidth = (float)kBarWidth / m_procMidi.timeSignature.beatsPerBar;

            GL.PushMatrix();
            GL.MultMatrix(parentBars ? transform.localToWorldMatrix : Matrix4x4.identity);
            GL.Begin(GL.LINES);
            m_lineMat.SetPass(0);

            Color MainLine = m_playState != EPlayState.FAIL ? Color.white : Color.red,
                subLine01 = m_playState != EPlayState.FAIL ? Color.grey : (Color.red * Color.grey),
            subLine02 = m_playState != EPlayState.FAIL ? Color.gray : (Color.red * Color.gray);

            float laneDivider = kLaneHeight / noteLanes;

            GL.Color(subLine02);
            for (uint ui = 1; ui < noteLanes; ui++)
            {
                start = new Vector3(0, -(barHeight) * 0.5f + (kLaneHeight * ui));
                end = new Vector3(m_procMidi.bars * kBarWidth, -(barHeight) * 0.5f + (kLaneHeight * ui));
                GL.Vertex(start);
                GL.Vertex(end);
            }

            for (int i = 0; i < m_procMidi.bars; i++)
            {
                start = new Vector3(kBarWidth * i, (barHeight) * 0.5f);
                end = new Vector3(kBarWidth * i, -(barHeight) * 0.5f);
                for (int j = 1; j < m_procMidi.timeSignature.beatsPerBar; j++)
                {
                    subStart = new Vector3(j * subBarWidth, 0);
                    subEnd = new Vector3(j * subBarWidth, 0);

                    GL.Color(subLine01);
                    GL.Vertex(subStart + start);
                    GL.Vertex(subEnd + end);

                }
                GL.Color(MainLine);
                GL.Vertex(start);
                GL.Vertex(end);
            }

            //Cap bar
            start = new Vector3(kBarWidth * m_procMidi.bars, (barHeight) * 0.5f);
            end = new Vector3(kBarWidth * m_procMidi.bars, -(barHeight) * 0.5f);
            GL.Vertex(start);
            GL.Vertex(end);

            start = new Vector3(0, (barHeight) * 0.5f);
            end = new Vector3(kBarWidth * m_procMidi.bars, (barHeight) * 0.5f);
            GL.Vertex(start);
            GL.Vertex(end);

            start = new Vector3(0, -(barHeight) * 0.5f);
            end = new Vector3(kBarWidth * m_procMidi.bars, -(barHeight) * 0.5f);
            GL.Vertex(start);
            GL.Vertex(end);

            GL.End();
            GL.PopMatrix();
        }

        void DebugSpawnLocations()
        {
            if (m_procMidi == null || m_debugMesh == null || m_debugMat == null)
                return;
            NoteSpawnInfo[] noteSpawn = m_procMidi.spawnInfo;
            Matrix4x4 matrix;
            Vector3 position;

            for (int i = 0; i < noteSpawn.Length; i++)
            {
                matrix = Matrix4x4.identity;
                noteSpawn[i].GetBarPosition(out position, kBarWidth, kLaneHeight, 0.5f);
                matrix.SetTRS(position - Vector3.up * ((kLaneHeight * noteLanes) * 0.5f), Quaternion.identity, Vector3.one);
                Graphics.DrawMesh(m_debugMesh, matrix, m_debugMat, 0);
            }
        }

        List<T> CastToDependecy<T>(MonoBehaviour[] source, List<T> list = null) where T : IInjectableDependancy
        {
            if (source == null)
                return null;
            if (list == null)
                list = new System.Collections.Generic.List<T>();
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == null)
                    continue;
                IInjectableDependancy dependacy = source[i] as IInjectableDependancy;
                if (dependacy != null && dependacy is T)
                {
                    T castObj = (T)(IInjectableDependancy)source[i];
                    if (castObj != null)
                        list.Add(castObj);
                }
            }
            return list;
        }

        Vector3 GetMousePosition()
        {
            Plane pointerPlane = new Plane(-Vector3.forward, Vector3.zero);

            float hit = 0;
            Vector3 camera2world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray camera2Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(camera2world, Vector3.forward * 100, Color.white);
            pointerPlane.Raycast(camera2Ray, out hit);
            return camera2Ray.GetPoint(hit);
        }

        void SpawnNotePellet(NoteSpawnInfo spawnInfo)
        {
            //Failure exit check.
            if (m_notePelletPrefab == null)
                return;

            //Initilaise local variables.
            GameObject newPellet;
            bool invertPolyphony = false;

            //Handle polyphonic notespawns
            if (spawnInfo.isPolyphonic())
            {
                //Handle long polyphonic notespawns
                //Disabled for now due to bugs.
                //if (spawnInfo.length >= kLongPelletMin) 
                //{
                //    CubicBezier bezier;
                //    float length = kBarWidth * spawnInfo.length;
                //    int direction = invertPolyphony ? -1 : 1;
                //    float dy = spawnInfo.GetRange() * kLaneHeight * direction;
                    
                //    Vector3 A = Vector3.zero, B = Vector3.zero;
                //    B.x += length;
                //    B.y += dy;

                //    bezier = new CubicBezier(A, B);
                //    newPellet = new GameObject("Poly long Pellet");
                //    newPellet.AddCubicBezierComponents(bezier,null);
                //}
                //Handle short polyphonic notespawns
                //else
                //{
                    newPellet = Instantiate(m_polyPelletPrefab);
                    PolyPhonicNotePellet ppnp = newPellet.GetComponent<PolyPhonicNotePellet>();
                    if (ppnp != null)
                    {
                        int range;
                        bool useRange = spawnInfo.RangeBetweenLastAndNext(out range);
                        const int ups = 24, cloneEveryLanes = 6;
                        invertPolyphony = useRange ? Mathf.Sign(range) < 0 : !invertPolyphony;
                        ppnp.Initialise(Vector3.up * -0.5f, Vector3.up * spawnInfo.GetHighest() * (kLaneHeight * 0.5f), ups, spawnInfo.GetRange() / cloneEveryLanes, invertPolyphony);
                    }
                //}
            }
            //Handle Monophonic notespawns
            //Handle long monophonic notespawns
            else if (spawnInfo.length >= kLongPelletMin)
            {
                newPellet = new GameObject();

                BoxCollider bc = newPellet.AddComponent<BoxCollider>();
                NotePelletLong npl = newPellet.AddComponent<NotePelletLong>();
                float length = kBarWidth * spawnInfo.length;
                Mesh mesh = BuildLongPelletMesh(length);
                npl.Initialise(mesh, m_debugMat, bc, length);
            }
            //Handle short monophonic notespawns
            else
            {
                newPellet = Instantiate(m_notePelletPrefab);
            }
            
            //Translate gameobject.
            Vector3 position;
            spawnInfo.GetBarPosition(out position, kBarWidth, kLaneHeight, 0.5f);
            newPellet.transform.position = position - Vector3.up * ((kLaneHeight * noteLanes) * 0.5f);
            NotePellet pelletMono = newPellet.GetComponent<NotePellet>();

            //Teleportation check.
            bool AllowTeleport = !(pelletMono is PolyPhonicNotePellet || pelletMono is NotePelletLong) 
                && Mathf.Abs(spawnInfo.RangeBetweenThisAndNext()) >= 12 && spawnInfo.next != null;

            //Add note pellet reference to list and inject dependencies.
            m_notePellets.Add(pelletMono);
            pelletMono.InjectDependancy(this); //Dependency injection.

            //Set up teleportation values.
            if (AllowTeleport)
            {
                Vector3 nextPosition;
                spawnInfo.next.GetBarPosition(out nextPosition, kBarWidth, kLaneHeight, 0.5f);
                nextPosition.x = position.x;
                Vector3 teleportTarget = nextPosition - (Vector3.up * ((kLaneHeight * noteLanes) * 0.5f));
                pelletMono.SetAllowTeleportWithDestination(AllowTeleport,teleportTarget);
            }
        }

        void OnPlayerFireShots(int shots)
        {
            if (m_killZone == null || m_playerShotPrefab == null)
                return;
            int iterator = 0, heightMultiplier = 1, sign = 1;
            const float height = 1f;
            while (iterator < shots)
            {
                GameObject go = Instantiate(m_playerShotPrefab);
                PlayerShot playerShot = go.GetComponent<PlayerShot>();
                if (playerShot != null)
                {
                    playerShot.Initialise(new QuadraticBezierTransform(m_player.transform,m_killZone.transform,height*heightMultiplier * sign),m_player.transform.position);
                    horizontalMovementHandler.Add(playerShot);
                    m_playerShots.Add(playerShot);
                    playerShot.InjectDependancy(this);
                }
                iterator++;
                sign *= -1;
                if(iterator % 2 == 0)
                    heightMultiplier++;
            }
        }

        float DistanceBetweenKillzoneAndPlayer()
        {
            if (m_killZone == null || m_player == null)
                return 0;
            return m_killZone.GetLooktargetPosition().x - m_player.transform.position.x;
        }

        Mesh BuildLongPelletMesh(float length)
        {
            Mesh mesh = new Mesh();
            mesh.hideFlags = HideFlags.HideAndDontSave;

            Vector3[] verts = new Vector3[4]
			{
				new Vector3(0,-0.5f,0),
				new Vector3(0, 0.5f,0),
				new Vector3(length,-0.5f,0),
				new Vector3(length,0.5f,0)
			};

            Color[] colours = new Color[4];

            for (int i = 0; i < 4; i++)
            {
                colours[i] = Color.white;
            }

            Vector2[] uvs = new Vector2[4]
			{
				Vector2.zero,
				Vector2.up,
				Vector2.right,
				Vector2.one
			};

            int[] triangles = new int[6]
			{
				0,1,2,
				2,1,3
			};

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.colors = colours;
            mesh.RecalculateBounds();

            return mesh;
        }

        Mesh BuildLongPelletPolyPhonic(Vector3 A, Vector3 B)
        {
           return null;
        }

        void OnPlayerTeleport(Vector3 destination, Vector3 delta)
        {
            m_cameraControl.transform.position += delta;
        }

        void OnFailureConditionMet()
        {
            ChangeState(EPlayState.FAIL);
            m_mainGameControl.ChangeState(EGameState.MainMenu, null);
        }

        void OnWinConditionMet()
        {
            ChangeState(EPlayState.SUCCESS);
            m_mainGameControl.ChangeState(EGameState.MainMenu, null);
        }

        void EnableDependencies(bool value)
        {
            if (m_player != null)
                m_player.gameObject.SetActive(value);
            if (m_killZone != null)
            {
                if (m_playState == EPlayState.PLAY || !value)
                    m_killZone.gameObject.SetActive(value);
            }
            if (m_cameraControl != null)
                m_cameraControl.enabled = false;
        }

        void ChangeState(EPlayState newState)
        {
            if (m_playState == newState)
                return;
            switch (newState)
            {
                case EPlayState.AWAITING_START:
                    break;
                case EPlayState.AWAITING_PLAY:
                    break;
                case EPlayState.SUCCESS:
                    m_musicPlayer.Stop();
                    break;
                case EPlayState.PLAY:
                    if (m_musicPlayer.isPlaying)
                    {
                        m_musicPlayer.UnPause();
                        break;
                    }
                    m_musicPlayer.Play();
                    break;
                case EPlayState.PAUSE:
                    m_musicPlayer.Pause();
                    break;
                case EPlayState.FAIL:
                    m_musicPlayer.Stop();
                    break;
            }
            m_playState = newState;
        }

        void ClearPelletList()
        {
            for (int i = m_notePellets.Count - 1; i >= 0; i--)
            {
                NotePellet pellet = m_notePellets[i];
                m_notePellets.Remove(pellet);
                Destroy(pellet.gameObject);
            }
            m_notePellets = null;
        }

        void ClearPlayerShotList()
        {
            for (int i = m_playerShots.Count - 1; i >= 0; i--)
            {
                PlayerShot shot = m_playerShots[i];
                m_playerShots.Remove(shot);
                horizontalMovementHandler.Remove(shot);
                if(shot != null)
                    Destroy(shot.gameObject);
            }
            m_notePellets = null;
        }
    }
}
