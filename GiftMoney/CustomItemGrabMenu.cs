using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace GiftMoney;

public class CustomItemGrabMenu : ItemGrabMenu
{
	private ClickableComponent sendMoneyButton;

	public static bool menuActive;

	private int UIWidth = 300;

	private int UIHeight = 200;

	public CustomItemGrabMenu(IList<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, behaviorOnItemSelect behaviorOnItemSelectFunction, string message)
		: base(inventory, reverseGrab, showReceivingMenu, (InventoryMenu.highlightThisItem)Utility.highlightSantaObjects, behaviorOnItemSelectFunction, message, (behaviorOnItemSelect)null, false, false, true, true, false, 0, (Item)null, -1, (object)null)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		sendMoneyButton = new ClickableComponent(new Rectangle(xPositionOnScreen - 2, yPositionOnScreen + 120, UIWidth - 32, 100), "Gift Money");
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (sendMoneyButton.containsPoint(x, y))
		{
			Game1.activeClickableMenu = new SendMoneyUI(Game1.currentLocation.currentEvent.secretSantaRecipient.Name, winterStarGift: true);
		}
		receiveRightClick(x, y, playSound);
	}

	public override void draw(SpriteBatch b)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
		Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        Game1.drawDialogueBox(xPositionOnScreen - 20, yPositionOnScreen + 50, UIWidth, UIHeight, speaker: false, drawOnlyBox: true);
		Utility.drawTextWithShadow(b, sendMoneyButton.name, Game1.dialogueFont, new Vector2((float)(sendMoneyButton.bounds.X + 35), (float)(sendMoneyButton.bounds.Y + 35)), Color.Black);
		base.draw(b, drawUpperPortion: false);
		drawMouse(b);
	}
}
