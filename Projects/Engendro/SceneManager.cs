using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// SceneManager
    /// </summary>
    public sealed class SceneManager : GameObject
    {
        #region Private fields

        private readonly List<Scene> scenes = [];
        private bool isChangingScene;

        #endregion

        #region Constructor

        // Constructor
        internal SceneManager(EngendroGame game)
            : base(game)
        {
        }

        #endregion

        #region Private members

        // PopCore
        private Scene PopCore()
        {
            if (scenes.Count == 0)
                throw new InvalidOperationException("Stack is empty.");

            if (isChangingScene)
                throw new InvalidOperationException("A push or pop task is already in progress.");

            isChangingScene = true;
            try
            {
                var scene = scenes[0];

                RemoveSceneCore(scene);

                if (scene.PausePreviousScenes)
                {
                    for (var i = 0; i < scenes.Count; i++)
                    {
                        scenes[i].Resume();
                    }
                }

                PreviousScene = scene;
                InputManager.Reset();
                return scene;
            }
            finally
            {
                isChangingScene = false;
            }
        }

        // RemoveSceneCore
        private bool RemoveSceneCore(Scene scene)
        {
            var index = scenes.IndexOf(scene);
            if (index == -1)
                return false;

            scenes.RemoveAt(index);
            scene.Deactivate();
            scene.UnloadContent();

            return true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (CurrentScene == null)
                return;

            if (CurrentScene.ExclusiveDraw)
            {
                CurrentScene.Draw(gameTime);
            }
            else
            {
                for (var i = scenes.Count - 1; i >= 0; i--)
                {
                    scenes[i].Draw(gameTime);
                }
            }

            TransitionManager.CurrentTransition.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (CurrentScene == null)
                return;

            TransitionManager.CurrentTransition.Update(gameTime);

            CurrentScene.HandleInput(gameTime);

            var sceneListSize = scenes.Count;
            var canUpdate = true;
            for (var i = 0; i < scenes.Count; i++)
            {
                scenes[i].ContinuousUpdate(gameTime);
                if (sceneListSize > scenes.Count)
                    break;

                if (canUpdate)
                {
                    if (i == 0)
                    {
                        scenes[i].Update(gameTime);
                    }
                    else if (!scenes[i - 1].PausePreviousScenes)
                    {
                        scenes[i].Update(gameTime);
                    }
                    else
                    {
                        canUpdate = false;
                    }

                    if (sceneListSize > scenes.Count)
                        break;
                }
            }
        }

        #endregion

        // Clear
        public void Clear()
        {
            while (scenes.Count > 0)
            {
                Pop();
            }
        }

        // Contains
        public bool Contains<T>() where T : Scene
        {
            for (var i = 0; i < scenes.Count; i++)
            {
                if (scenes[i] is T)
                    return true;
            }

            return false;
        }

        // Contains
        public bool Contains(Scene scene)
        {
            return scenes.Contains(scene);
        }

        // CurrentScene
        public Scene? CurrentScene => scenes.Count == 0 ? null : scenes[0];

        // GetScenes
        public Scene[] GetScenes()
        {
            return [.. scenes];
        }

        // Pop
        public Scene Pop()
        {
            var result = PopCore();
            CurrentScene?.Activate();
            return result;
        }

        // PopUntil
        public void PopUntil(Scene scene)
        {
            while (CurrentScene != null)
            {
                if (CurrentScene == scene)
                    break;
                else
                    PopCore();
            }
            CurrentScene?.Activate();
        }

        // PreviousScene
        public Scene? PreviousScene { get; private set; }

        // Push
        public void Push(Scene scene)
        {
            if (scenes.Contains(scene))
                throw new ArgumentException("Scene already pushed.");

            if (isChangingScene)
                throw new InvalidOperationException("A push/pop task is currently in progress.");

            isChangingScene = true;
            try
            {
                if (scenes.Count > 0)
                {
                    if (scene.PausePreviousScenes)
                    {
                        for (var i = 0; i < scenes.Count; i++)
                        {
                            scenes[i].Pause();
                        }
                    }
                }

                PreviousScene = CurrentScene;
                PreviousScene?.Deactivate();
                Game.ResetElapsedTime();
                scenes.Insert(0, scene);
                scene.Initialize();
                scene.LoadContent();
                scene.Activate();
            }
            finally
            {
                isChangingScene = false;
            }
        }

        // Remove
        public bool Remove(Scene scene)
        {
            if (scene.IsCurrentScene)
            {
                Pop();
                return true;
            }
            else
                return RemoveSceneCore(scene);
        }

        // SceneCount
        public int SceneCount => scenes.Count;
    }
}
