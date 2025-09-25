#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GiftMoney.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace GiftMoney;

public class SendMoneyUI : IClickableMenu
{
	private int UIWidth = 532;

	private int UIHeight = 400;

	private int PosX;

	private int PosY;

	private ClickableComponent TitleLabel;

	private TextBox textBox;

	private ClickableTextureComponent okButton;

	public static int amount;

	public static List<string> npcNames = new List<string>();

	private Button lovedButton;

	private Button likedButton;

	private Button neutralButton;

	private string npcName { get; set; }

	public SendMoneyUI(string NpcName, bool winterStarGift = false)
	{
        //IL_009b: Unknown result type (might be due to invalid IL or missing references)
        //IL_00d0: Unknown result type (might be due to invalid IL or missing references)
        //IL_015b: Unknown result type (might be due to invalid IL or missing references)
        //IL_016e: Unknown result type (might be due to invalid IL or missing references)

        Vector2 center = Utility.getTopLeftPositionForCenteringOnScreen(UIWidth, UIWidth);
        base.initialize((int)center.X, (int)center.Y, UIWidth, UIHeight);

        npcName = NpcName;

		lovedButton = new Button(I18n.Loved(), delegate
		{
			triggerGift(npcName, GiftType.Loved);
		}, GiftType.Loved, 10000);
		lovedButton.SetPosition(xPositionOnScreen + this.width / 2 - lovedButton.width / 2, yPositionOnScreen + height / 4 + 15);

		likedButton = new Button(I18n.Liked(), delegate
		{
			triggerGift(npcName, GiftType.Liked);
		}, GiftType.Liked, 1000);
		likedButton.SetPosition(xPositionOnScreen + this.width / 2 - likedButton.width / 2, lovedButton.bounds.Y + 80);

        neutralButton = new Button(I18n.Neutral(), delegate
		{
			triggerGift(npcName, GiftType.Neutral);
		}, GiftType.Neutral, 100);
        neutralButton.SetPosition(xPositionOnScreen + this.width / 2 - neutralButton.width / 2, likedButton.bounds.Y + 80);

	}

	public override void update(GameTime gameTime)
	{
		base.update(gameTime);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{

		/*
		if (winterStarGift)
		{
			triggerGift(Game1.getCharacterFromName(npcName), int.Parse(textBox.Text));
			return;
		}
		*/

		if (lovedButton.containsPoint(x, y))
			lovedButton.CallEvent();

		Debug.WriteLine("HEllo");
	}

    public override void performHoverAction(int x, int y)
    {
        lovedButton.PerformHover(x, y);
        lovedButton.bounds.X = xPositionOnScreen + this.width / 2 - lovedButton.width / 2;

		likedButton.PerformHover(x, y);
		likedButton.bounds.X = xPositionOnScreen + this.width / 2 - likedButton.width / 2;

		neutralButton.PerformHover(x, y);
        neutralButton.bounds.X = xPositionOnScreen + this.width / 2 - neutralButton.width / 2;
    }

    public override void draw(SpriteBatch b)
	{
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
        lovedButton.draw(b);
		likedButton.draw(b);
		neutralButton.draw(b);
		drawMouse(b);
	}

	private void triggerGift(string npcName, GiftType type)
	{

		NPC npc = Game1.getCharacterFromName(npcName);

		if (!Game1.player.friendshipData.ContainsKey(npc.Name))
		{
			Game1.activeClickableMenu = new DialogueBox(I18n.Misc_NPCNotMet());
			return;
		}
		Friendship friendship = Game1.player.friendshipData[npc.Name];
		int giftsThisWeek = friendship.GiftsThisWeek;
		int giftsToday = friendship.GiftsToday;
		if (giftsThisWeek >= 2)
		{
			Game1.activeClickableMenu = new DialogueBox(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3987", npc.displayName, 2));
			return;
		}
		if (giftsToday > 0 || npcNames.Contains(npc.Name))
		{
			Game1.activeClickableMenu = new DialogueBox(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3981", npc.displayName));
			return;
		}
		if (amount > Game1.player.Money)
		{
			Game1.activeClickableMenu = new DialogueBox(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
			return;
		}
        StardewValley.Object lovedGift = new StardewValley.Object(Vector2.Zero, ModEntry.GiftIds.Loved);
        StardewValley.Object likedGift = new StardewValley.Object(Vector2.Zero, ModEntry.GiftIds.Liked);
        StardewValley.Object neutralGift = new StardewValley.Object(Vector2.Zero, ModEntry.GiftIds.Neutral);

		npcNames.Add(npc.Name);

		StardewValley.Object targetItem = null;

		switch (type)
		{
			case GiftType.Loved:
				targetItem = lovedGift;
				break;
			case GiftType.Liked:
				targetItem = likedGift;
				break;
			case GiftType.Neutral:
				targetItem = neutralGift;
				break;
		}
		npc.receiveGift(targetItem, Game1.player);
		Game1.player.Money -= amount;
	}
}
