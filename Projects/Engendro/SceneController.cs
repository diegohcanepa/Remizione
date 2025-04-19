namespace Engendro
{
    /// <summary>
    /// SceneController
    /// </summary>
    public sealed class SceneController
    {
        private readonly Scene scene;

        // Constructor
        internal SceneController(Scene scene)
        {
            this.scene = scene;
        }

        // ExclusiveDraw
        public bool ExclusiveDraw { get; set; }

        // IsPushed
        public bool IsPushed => scene.Game.SceneManager.Contains(scene);

        // PausePreviousScenes
        public bool PausePreviousScenes { get; set; }

        // Pop
        public bool Pop()
        {
            if (scene.IsCurrentScene)
            {
                scene.Game.SceneManager.Pop();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Push
        public bool Push()
        {
            if (!scene.IsCurrentScene)
            {
                scene.Game.SceneManager.Push(scene);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Remove
        public bool Remove()
        {
            if (scene.IsCurrentScene)
            {
                scene.Game.SceneManager.Pop();
                return true;
            }
            else
            {
                return false;
            }
        }

        // TransitionAware
        public bool TransitionAware { get; set; } = true;
    }
}
