using Engendro;

namespace Remizione
{
    /// <summary>
    /// SessionOverlays
    /// </summary>
    public sealed class SessionOverlays
    {
        #region Private fields

        private readonly BonfireScene bonfireScene;
        private readonly EchoScene echoScene;
        private readonly InventoryScene inventoryScene;
        private readonly ItemInfoScene itemInfoScene;
        private readonly MiniMapScene miniMapScene;
        private readonly NarrationScene narrationScene;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public SessionOverlays(GameSession session)
        {
            this.session = session;
            this.echoScene = new EchoScene(session);
            this.narrationScene = new NarrationScene(session);
            this.inventoryScene = new InventoryScene(session.PlayerData.Inventory);
            this.miniMapScene = new MiniMapScene(session);
            this.itemInfoScene = new ItemInfoScene(session);
            this.bonfireScene = new BonfireScene();
        }

        #endregion

        #region Private members

        // PushScene
        private void PushScene<T>(Scene scene, bool stopPlayer = false) where T : Scene
        {
            if (stopPlayer && session.Player != null)
            {
                session.Player.StopMoving();
                session.Camera.FocusTarget();
            }

            if (session.Game.SceneManager.CurrentScene is not T)
                session.Game.SceneManager.Push(scene);
        }

        #endregion

        // ShowBonfire
        public void ShowBonfire()
        {
            PushScene<BonfireScene>(bonfireScene);
        }

        // ShowDialogMenu
        public void ShowDialogMenu(DialogBlock dialogBlock)
        {
            session.DialogOptionId = -1;
            var scene = new DialogBlockScene(session, dialogBlock);
            session.Game.SceneManager.Push(scene);
        }


        // ShowEcho
        public void ShowEcho(string text)
        {
            PushScene<EchoScene>(echoScene);
            echoScene.Show(text);
        }

        // ShowEpigraph
        public void ShowEpigraph(string text)
        {
            if (session.Game.SceneManager.CurrentScene is not EpigrahScene)
            {
                var scene = new EpigrahScene();
                session.Game.SceneManager.Push(scene);
                scene.Show(text);
            }
        }

        // ShowInventory
        public void ShowInventory()
        {
            PushScene<InventoryScene>(inventoryScene);
        }

        // ShowItemInfo
        public void ShowItemInfo(Item item)
        {
            itemInfoScene.Show(item);
            PushScene<ItemInfoScene>(itemInfoScene);
        }

        // ShowMiniMap
        public void ShowMiniMap()
        {
            PushScene<MiniMapScene>(miniMapScene, true);
        }

        // ShowNarration
        public void ShowNarration(string text, string soundName)
        {
            PushScene<NarrationScene>(narrationScene);
            narrationScene.Show(text, soundName);
        }
    }
}