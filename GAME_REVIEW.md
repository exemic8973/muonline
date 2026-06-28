# MuOnline Client — Game Review (Language & Skills)

> Reviewer: Chief Game Officer pass
> Scope: Chinese/localization display, skill display/usage, quick-cast, potion quick-slots
> Build note: the cloud review environment has no .NET SDK, so changes were verified by
> inspection. **Please run a local build of the head you use (e.g. MuWinDX) to confirm.**

---

## 1. Executive summary

The reported symptoms — "Chinese won't display", "skills don't show / can't be used /
no quick-cast / no potion quick-setup" — trace back to **three concrete defects** plus a
set of design limitations:

| # | Severity | Defect | Effect on player |
|---|----------|--------|------------------|
| 1 | 🔴 Blocker | `Content/lang_en.json` & `Content/lang_zh.json` are listed as `EmbeddedResource` in `Client.Main.csproj` but **the files did not exist** | Every platform head fails to compile → you can only run a stale binary |
| 2 | 🟠 High | `GraphicsManager` loaded the `CN` font with **no fallback**, and `CN.spritefont` points at a Windows-only absolute path (`C:\Windows\Fonts\msyh.ttc`) | If the font fails to build/load the whole game crashes on boot; non-Windows heads can't build the font at all |
| 3 | 🟠 High | Skill names are read from the bundled **Korean** `skill.bmd` (EUC-KR). The `CN` font has Latin + CJK-Han glyphs but **no Korean (Hangul) glyphs** | Skill names render as `?`/boxes — "skills not displaying right" |
| 4 | 🟡 Medium | The skill quick-slot only auto-selected a skill **at construction time**; skills arrive from the server *after* login | If the skill list arrives late, nothing is selected → right-click cast does nothing |

Items 1–4 are **fixed in this branch**. Quick-cast and potion quick-slots were already
implemented in code (see §4) and are wired correctly; they needed the above blockers
resolved to actually function.

---

## 2. Fixes applied in this PR

### Fix 1 — Restore the missing localization files (unblocks the build)
- Added `Client.Main/Content/lang_en.json` and `Client.Main/Content/lang_zh.json`.
- They contain every key currently consumed via `Loc.Get(...)` (the 5 `CharInfo_*` stat
  labels) plus a seed set for the skill panel and common buttons.
- Logical resource names match `Loc.cs`'s lookup (`Client.Main.Content.lang_zh.json`).

### Fix 2 — Crash-proof font loading + fallback chain
- `GraphicsManager.Init` now calls `LoadFontWithFallback("CN")` which tries
  **CN → NotoKR → Arial**, logs which font was used, and throws a *clear* error only if
  none load. A missing/failed `CN.xnb` no longer takes down the whole client.

### Fix 3 — Reliable, localized skill names
- `SkillDatabase.GetSkillName` now prefers **curated localized names**:
  Chinese table (`s_namesZh`, 70+ skills) when language = `zh`, then the English table,
  and only falls back to the BMD name if it is *clean printable text*
  (`IsDisplayableBmdName` rejects control chars / `�` mojibake).
- Net effect: skill names always render in a glyph set the font actually contains.
- Skill selection panel strings (`Select Skill`, `Skill Info`, type labels, hover hint)
  now route through `Loc.Get`, so they localize with the rest of the UI.

### Fix 4 — Auto-select first skill when the list arrives late
- `SkillQuickSlot.Update` now selects the first available skill if none is selected yet,
  so right-click casting works as soon as the server delivers the skill list — no need to
  open the F3 panel first.

---

## 3. How the language pipeline now works (and its limits)

There are **two independent text sources**, and they must not be confused:

1. **Client UI chrome** (stat labels, panels, buttons) → `Loc.Get(key)` →
   `lang_zh.json` / `lang_en.json`. Toggle at runtime via the Pause-menu language switch
   (`Loc.CurrentLanguage = "zh"|"en"`). This is now fully functional for the wired keys.
   *Most* UI strings are still hardcoded English — see recommendation R3.

2. **Game data names** (skills, items, maps) → parsed from MU asset files in `Client.Data`.
   The bundled assets are the **Korean** client's, so these are EUC-KR. Skill names are now
   overridden by the curated tables; **item/map/NPC names are still Korean** unless a
   Chinese data set is supplied (recommendation R2).

3. **Server-originated text** (chat, system/drop messages) → sent by OpenMU, default
   **English**. The client renders whatever bytes the server sends; making these Chinese is
   a *server-side* change (localize OpenMU message strings), not a client change.

### The font (read this before "Chinese still looks wrong")
`CN.spritefont` builds a **single** glyph atlas from `msyh.ttc` covering ASCII + the full
CJK Han block (U+4E00–U+9FA5, ~20,900 glyphs). On a desktop GPU this packs into roughly a
4096×4096 texture and loads, but:
- The **absolute Windows path** means only Windows heads can build it. Linux/Mac/Android/iOS
  content builds will fail on this asset.
- A single static SpriteFont is the wrong long-term tool for full CJK (atlas size, memory,
  and it can never cover characters outside the baked range). See recommendation R1.

---

## 4. Skills / quick-cast / potions — current behavior (verified by code read)

- **Cast a skill:** select one in the centered bottom skill slot or press **F3** to open the
  selection panel (now auto-selects the first skill). **Right-click** a monster/target to
  cast. Range, mana/AG cost, cooldown, SafeZone and pathing-to-target are all handled in
  `GameSceneSkillController`.
- **Quick-cast bar (Q/W/E/A/S/1/2/3):** `QuickSlotOverlay` puts invisible hit-areas over the
  decorative bottom bar (virtual 1280×720 coords).
  - **Ctrl + key** = bind the currently selected skill to that slot.
  - **key** = use the slot (skill slot re-selects that skill; potion slot drinks it).
- **Potion quick-setup:** drag a consumable (item group 13/14) from the inventory onto a
  Q/W/E/A/S/1/2/3 slot → `InventoryControl` calls `QuickSlotOverlay.RegisterPotion`; pressing
  the key sends `SendConsumeItemRequestAsync`.

These paths are correctly wired. If they still feel unresponsive in-game after this PR,
the most likely remaining causes are: (a) the slot hit-rectangles not lining up with your
actual bottom-bar texture, or (b) the character genuinely having no skills/potions yet
(add them via the OpenMU admin panel — see `MEMO.md`).

---

## 5. Recommendations (not done here — need product/testing decisions)

- **R1 — Adopt a dynamic font (FontStashSharp).** Replace the static CJK SpriteFont with a
  runtime TTF rasterizer so *any* Chinese (chat, names, server text) renders without baking
  a giant atlas, and so all platform heads build. This is the proper fix for full CJK.
- **R2 — Ship/define a Chinese data set or name tables for items/maps/NPCs**, the same way
  this PR did for skills, so non-skill names stop showing Korean.
- **R3 — Route the remaining hardcoded UI strings through `Loc.Get`** and grow the
  `lang_*.json` dictionaries. Today only the character-info stat labels are localized.
- **R4 — Bundle an open-source CJK font** (e.g. Noto Sans SC) at a relative path instead of
  `C:\Windows\Fonts\msyh.ttc`, so the content pipeline builds on every OS / CI.
- **R5 — Server-side localization** for chat/system/drop messages if Chinese is desired
  there (OpenMU message resources).
- **R6 — Make quick-slot bindings persist** (per-character save) and add an on-screen key
  hint, to improve discoverability of the potion/skill quick bar.

---

## 6. Test checklist for the developer (Windows)

```bat
dotnet tool restore
dotnet build .\MuWinDX\MuWinDX.csproj -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
```
1. Game boots (no font/embedded-resource exception).
2. Character window (C) shows 力量/敏捷/体力/能量/统率 with language = zh.
3. F3 skill panel title/labels are Chinese; skill names are Chinese (not boxes).
4. Right-click a monster casts the selected skill.
5. Drag a potion onto a Q/W/E/1 slot, press the key → potion is consumed.
