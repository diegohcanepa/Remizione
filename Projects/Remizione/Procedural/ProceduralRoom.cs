using Engendro;
using EngendroAdventure.Scripting;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public sealed class ProceduralRoom : GameRoom
    {
        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            AtlasName = string.Empty;
            LightingSystem = false;
            WorldManager = new WorldManager(session, new Size(Screen.NativeWidth, Screen.NativeHeight), 101);
        }

        // Regenerate
        private void Regenerate()
        {
            Children.Clear();
            RemoveWalkArea("");

            CustomWidth = WorldManager.GridSize * WorldManager.BlockSize.Width;
            CustomHeight = WorldManager.GridSize * WorldManager.BlockSize.Height;

            // Define walk area
            var vertices = WorldManager.GetWalkareaVertices();
            AddWalkArea("", vertices);

            // Add existing blocks
            for (var i = 0; i < WorldManager.Blocks.Count; i++)
            {
                Children.Add(WorldManager.Blocks[i]);
                foreach (var prop in WorldManager.Blocks[i].Props)
                {
                    Children.Add(prop);
                }
            }

            if (Session.Player != null)
                Children.Add(Session.Player);
        }

        #region Protected members

        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Session.IsNewSession)
                WorldManager.AddBlock(new(WorldManager.GridSize / 2), true);

            Regenerate();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (Session.Player != null)
            {
                Session.Player.Position = WorldManager.Blocks[0].BoundingBox.Center;
                Session.Camera.FollowTarget(Session.Player);
            }
        }

        #endregion

        // Expand
        [ScriptMethod]
        public void Expand()
        {
            if (Session.Player != null)
            {
                if (WorldManager.GetBlockFromScreen(Session.Player.Position) is WorldBlock terrainBlock)
                {
                    var newBlock = terrainBlock.Expand(EngendroAdventure.Direction.Up);
                    if (newBlock != null)
                        Children.Add(newBlock);

                    newBlock = terrainBlock.Expand(EngendroAdventure.Direction.Right);
                    if (newBlock != null)
                        Children.Add(newBlock);

                    newBlock = newBlock.Expand(EngendroAdventure.Direction.Down);
                    if (newBlock != null)
                        Children.Add(newBlock);

                    newBlock = terrainBlock.Expand(EngendroAdventure.Direction.Left);
                    if (newBlock != null)
                        Children.Add(newBlock);

                    Regenerate();
                }
            }
        }

        // WorldManager
        public WorldManager WorldManager { get; }
    }
}
