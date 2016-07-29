using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeFour
{
    //Base interface
    public interface IInjectableDependancy { }

    public interface IMIDIProcessor : IInjectableDependancy
    {
        ProcessedMIDI ProcessMIDI(MIDI midi, int track = -1);
    }

    public interface IRequireIntialisation
    {
        void Initialise();
    }

    public interface IRequireIntialisation<T>
    {
        void Initialise(T[] parametres);
    }

    public interface IRequireSingleIntialisation<T>
    {
        void Initialise(T parametre1);
    }

    public interface IRequireIntialisation<T1,T2>
    {
        void Initialise(T1[] parametres1, T2[] parametres2);
    }

    public interface IRequireSingleIntialisation<T1, T2>
    {
        void Initialise(T1 parametre1, T2 parametre2);
    }

    public interface IRequireSingleIntialisation<T1, T2, T3>
    {
        void Initialise(T1 parametre1, T2 parametre2, T3 parametre3);
    }

    public interface IHorizontalMovementHandler : IInjectableDependancy
    {
        void HandleHorizontalMovement(float delta);
    }

    public interface IMoveToDestination : IInjectableDependancy
    {
        void MoveTo(Vector3 destination, float delta, Constraint constrait);
    }

    public interface IHandleMouseDown : IInjectableDependancy
    {
        bool OnMouseDown(int i);
    }

    public interface INotePelletContainer : IInjectableDependancy
    {
        GameObject RemoveNotePellet(NotePellet pellet);
    }


    public interface IPlayerShotContainer : IInjectableDependancy
    {
        PlayerShot Remove(PlayerShot shot);
    }

    public interface IHorizontalMovementContainer : IInjectableDependancy
    {
        IHorizontalMovementHandler Remove(IHorizontalMovementHandler handler);
    }

    public interface IDependancyContainer : IInjectableDependancy
    {
        IInjectableDependancy Remove(IInjectableDependancy dependancy);
    }

    public interface IDependancyContainer<T> where T : IInjectableDependancy
    {
        T Remove(T dependancy);
    }

    public interface IRequireDependancy<T> where T : IInjectableDependancy
    {
        void InjectDependancy(T dependancy);
    }

    public interface IRequireCollider
    {
        Bounds GetBounds();
    }

    public interface ITransformBlend
    {
        void SetBlend(float f);
    }

    public interface IInjectType<T>
    {
        T RequestInjection();
    }
}
