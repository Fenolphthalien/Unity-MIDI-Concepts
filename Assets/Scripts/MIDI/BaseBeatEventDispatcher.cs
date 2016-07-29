using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityMIDI
{
    public abstract class BaseBeatEventDispatcher : MonoBehaviour
    {
        [SerializeField]
        protected List<IBeatEventHandler> m_subscribers = new List<IBeatEventHandler>();

        public bool sendMessages;

        void Awake()
        {
            SubcribeEventHandlersOnGameObject();
        }
        
        protected void SendOnBeat()
        {
            for (int i = 0; i < m_subscribers.Count; i++)
            {
                if (IsReferenceSelf(m_subscribers[i]))
                    continue;
                if (m_subscribers[i] == null)
                {
                    m_subscribers.Remove(m_subscribers[i]);
                    continue;
                }
                IBeatHandler handler = (IBeatHandler)m_subscribers[i];
                if (handler != null)
                    handler.OnBeat();
            }
            if (sendMessages)
                SendMessage("OnBeat");
        }

        protected void SendStrongBeat()
        {
            for (int i = 0; i < m_subscribers.Count; i++)
            {
                if (IsReferenceSelf(m_subscribers[i]))
                    continue;
                if (m_subscribers[i] == null)
                {
                    m_subscribers.Remove(m_subscribers[i]);
                    continue;
                }
                IStrongBeatHandler handler = m_subscribers[i] as IStrongBeatHandler;
                if (handler != null)
                    handler.OnStrongBeat();
            }
            if (sendMessages)
                SendMessage("OnStrongBeat");
        }

        bool IsReferenceSelf(IBeatEventHandler handler)
        {
            BaseBeatEventDispatcher checkObj = handler as BaseBeatEventDispatcher;
            return handler != null && checkObj == this;
        }

        void SubcribeEventHandlersOnGameObject()
        {
            IBeatEventHandler[] handlers = GetComponents<IBeatEventHandler>();
            if (handlers == null)
                return;
            m_subscribers.AddRange(handlers);
        }
    }
}