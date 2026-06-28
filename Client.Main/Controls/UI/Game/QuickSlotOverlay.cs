using System;
using System.Collections.Generic;
using Client.Main.Content;
using Client.Main.Controllers;
using Client.Main.Controls.UI.Game.Skills;
using Client.Main.Core.Client;
using Client.Main.Core.Utilities;
using Client.Main.Helpers;
using Client.Main.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client.Main.Controls.UI.Game
{
    /// <summary>
    /// Invisible interactive overlays on top of the decorative QWE/ASD/123 textures.
    /// No bar background — only item icons and hover highlights.
    /// </summary>
    public class QuickSlotOverlay : UIControl
    {
        public struct SlotDef
        {
            public readonly int Col, Row;
            public readonly Keys Key;
            public bool IsPotion;
            public SkillEntryState? Skill;
            public byte[]? PotionRawData;
            public Texture2D? Icon;

            public SlotDef(int col, int row, Keys key)
            {
                Col = col; Row = row; Key = key;
            }

            public bool IsEmpty => Skill == null && PotionRawData == null;

            public void Clear() { Skill = null; PotionRawData = null; Icon = null; }
        }

        // Slot layout (virtual 1280x720 coordinates)
        private static readonly (int X, int Y, int W, int H, Keys Key)[] s_slotLayout =
        {
            // Row 1 — QWE (decorative texture at 389,619,157x98)
            (389, 619, 52, 98, Keys.Q),      // col 0
            (441, 619, 50, 98, Keys.W),      // col 1
            (493, 619, 53, 98, Keys.E),      // col 2
            // Row 2 — AS (decorative texture at 545,642,85x75)
            (545, 640, 42, 77, Keys.A),      // col 3
            (587, 640, 43, 77, Keys.S),      // col 4
            // Row 3 — 123 (decorative texture at 711,621,156x97)
            (711, 621, 52, 97, Keys.D1),     // col 5
            (763, 621, 52, 97, Keys.D2),     // col 6  
            (815, 621, 52, 97, Keys.D3),     // col 7
        };

        private static readonly SlotDef[] s_slots;
        private static bool s_dirty;

        private SpriteFont _font;
        private Texture2D _pixel;

        static QuickSlotOverlay()
        {
            s_slots = new SlotDef[s_slotLayout.Length];
            for (int i = 0; i < s_slotLayout.Length; i++)
                s_slots[i] = new SlotDef(i, 0, s_slotLayout[i].Key);
        }

        public QuickSlotOverlay()
        {
            Interactive = true;
            AutoViewSize = false;
            ViewSize = new Point(UiScaler.VirtualSize.X, UiScaler.VirtualSize.Y);
            ControlSize = ViewSize;
        }

        public override async System.Threading.Tasks.Task Load()
        {
            _font = GraphicsManager.Instance.Font;
            _pixel = GraphicsManager.Instance.Pixel;
            await base.Load();
        }

        // ─── Public API ────────────────────────────────────────

        public static int GetSlotIndexAt(Point screenPos)
        {
            for (int i = 0; i < s_slotLayout.Length; i++)
            {
                var (x, y, w, h, _) = s_slotLayout[i];
                if (new Rectangle(x, y, w, h).Contains(screenPos))
                    return i;
            }
            return -1;
        }

        public static int GetSlotIndexByKey(Keys key)
        {
            for (int i = 0; i < s_slots.Length; i++)
                if (s_slots[i].Key == key)
                    return i;
            return -1;
        }

        /// <summary>Register a potion from inventory drag-drop.</summary>
        public static bool RegisterPotion(int slotIndex, byte[] rawData)
        {
            if (slotIndex < 0 || slotIndex >= s_slots.Length) return false;

            if (!ItemDatabase.TryGetItemGroupAndNumber(rawData, out byte g, out short _))
                return false;
            if (g != 14 && g != 13) return false; // consumable only

            s_slots[slotIndex].Clear();
            s_slots[slotIndex].PotionRawData = rawData;
            s_slots[slotIndex].IsPotion = true;

            var def = ItemDatabase.GetItemDefinition(rawData);
            if (!string.IsNullOrEmpty(def?.TexturePath))
            {
                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    var tex = await TextureLoader.Instance.PrepareAndGetTexture(def.TexturePath);
                    if (tex != null)
                        MuGame.ScheduleOnMainThread(() => { s_slots[slotIndex].Icon = tex; s_dirty = true; });
                });
            }
            return true;
        }

        /// <summary>Register a skill (from Ctrl+click or skill panel).</summary>
        public static void RegisterSkill(int slotIndex, SkillEntryState skill)
        {
            if (slotIndex < 0 || slotIndex >= s_slots.Length) return;
            s_slots[slotIndex].Clear();
            s_slots[slotIndex].Skill = skill;
            s_slots[slotIndex].IsPotion = false;
            s_dirty = true;
        }

        public static void ClearSlot(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < s_slots.Length)
                s_slots[slotIndex].Clear();
        }

        /// <summary>Use whatever is in the slot.</summary>
        public static void UseSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= s_slots.Length) return;
            ref var slot = ref s_slots[slotIndex];
            if (slot.IsEmpty) return;

            if (slot.Skill != null)
            {
                // Select the skill in the quick slot
                var dlg = FindQuickSlot();
                dlg?.SetSelectedSkill(slot.Skill.SkillId, slot.Skill.SkillLevel);
                SoundController.Instance.PlayBuffer("Sound/iButtonClick.wav");
                return;
            }

            if (slot.PotionRawData != null)
            {
                // Find the potion in inventory and consume it
                var net = MuGame.Network;
                var svc = net?.GetCharacterService();
                var state = net?.GetCharacterState();
                if (svc == null || state == null) return;

                var inv = state.GetInventoryItems();
                foreach (var kvp in inv)
                {
                    if (ItemDatabase.TryGetItemGroupAndNumber(kvp.Value, out byte g1, out short n1) &&
                        ItemDatabase.TryGetItemGroupAndNumber(slot.PotionRawData, out byte g2, out short n2) &&
                        g1 == g2 && n1 == n2)
                    {
                        _ = System.Threading.Tasks.Task.Run(async () =>
                        {
                            try { await svc.SendConsumeItemRequestAsync(kvp.Key); }
                            catch { /* ignore */ }
                        });
                        SoundController.Instance.PlayBuffer("Sound/iDrinkPotion.wav");
                        break;
                    }
                }
            }
        }

        // ─── Input ────────────────────────────────────────────

        public override bool OnClick()
        {
            return true; // consume clicks (don't propagate to controls beneath)
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Visible) return;
            HandleDropFromInventory();
        }

        private static Skills.SkillQuickSlot? FindQuickSlot() =>
            (MuGame.Instance?.ActiveScene as Scenes.GameScene)?.GetSkillQuickSlot();

        private void HandleDropFromInventory()
        {
            var mouse = MuGame.Instance.UiMouseState;
            var prev = MuGame.Instance.PrevUiMouseState;
            // Detect left-button release after dragging
            if (mouse.LeftButton != ButtonState.Released || prev.LeftButton != ButtonState.Pressed)
                return;

            if (GetSlotIndexAt(new Point(mouse.X, mouse.Y)) < 0)
                return;

            // The InventoryControl's HandleDropOutsideInventory already processed the drop.
            // Here we'd need the InventoryControl to register the item on us.
            // This is handled via the InventoryControl integration below.
        }

        // ─── Keyboard All-in-One ───────────────────────────────

        public static bool HandleKeyPress(Keys key, bool ctrl)
        {
            int idx = GetSlotIndexByKey(key);
            if (idx < 0) return false;

            if (ctrl)
            {
                // Ctrl+Key → register current selected skill into this slot
                var quickSlot = FindQuickSlot();
                var skill = quickSlot?.SelectedSkill;
                if (skill != null)
                    RegisterSkill(idx, skill);
                return true;
            }

            UseSlot(idx);
            return true;
        }

        // ─── Drawing ───────────────────────────────────────────

        public override void Draw(GameTime gameTime)
        {
            if (!Visible || _pixel == null) return;

            var mouse = MuGame.Instance.UiMouseState;
            var mousePt = new Point(mouse.X, mouse.Y);

            using var scope = new SpriteBatchScope(
                GraphicsManager.Instance.Sprite,
                SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None,
                RasterizerState.CullNone, null, UiScaler.SpriteTransform);

            var sb = GraphicsManager.Instance.Sprite;

            for (int i = 0; i < s_slots.Length; i++)
            {
                var (x, y, w, h, _) = s_slotLayout[i];
                var rect = new Rectangle(x, y, w, h);
                bool hovered = rect.Contains(mousePt);

                ref var slot = ref s_slots[i];

                if (slot.Icon != null)
                {
                    // Item icon
                    int pad = 4;
                    sb.Draw(slot.Icon, new Rectangle(x + pad, y + pad, w - pad * 2, h - pad * 2), Color.White);
                }
                else if (slot.Skill != null)
                {
                    // Skill icon placeholder — show skill name abbreviated
                    string name = SkillDatabase.GetSkillName(slot.Skill.SkillId);
                    if (_font != null && name != null)
                    {
                        var sz = _font.MeasureString(name) * 0.4f;
                        sb.DrawString(_font, name.Length > 4 ? name[..4] : name,
                            new Vector2(x + (w - sz.X) / 2, y + (h - sz.Y) / 2),
                            new Color(200, 200, 220, 200), 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                    }
                }

                if (hovered && !slot.IsEmpty)
                {
                    // Subtle glow border
                    Color glow = new(120, 160, 255, 80);
                    sb.Draw(_pixel, new Rectangle(x, y, w, 2), glow);
                    sb.Draw(_pixel, new Rectangle(x, y + h - 2, w, 2), glow);
                    sb.Draw(_pixel, new Rectangle(x, y, 2, h), glow);
                    sb.Draw(_pixel, new Rectangle(x + w - 2, y, 2, h), glow);
                }
            }
        }
    }
}
