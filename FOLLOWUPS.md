# Language & Skills — Follow-up Work Spec

> Owner of this doc: review/architecture pass (CGO).
> Implementation: performed by the implementing agent on its own branch/PR.
> Each item below is sized to be a single PR with explicit acceptance criteria so it
> can be reviewed against this spec. Work them **top-down by priority**.

Context: PR #1 (merged) fixed the build-break (untracked `lang_*.json`), made font loading
crash-proof, and replaced Korean skill names with curated zh/en tables. These follow-ups
finish the job described in `GAME_REVIEW.md` §5.

---

## P0 — F1: Dynamic CJK font (FontStashSharp)

**Why:** The current `CN.spritefont` bakes ASCII + the full CJK Han block into one static
atlas from `C:\Windows\Fonts\msyh.ttc`. It is Windows-path-locked (non-Windows heads can't
build it) and can never cover characters outside the baked range (chat, names, server text).
This is the root limitation behind "Chinese won't fully display".

**Approach:**
- Add `FontStashSharp.MonoGame` package to `Client.Main` (and ensure heads restore it).
- Bundle an OSS CJK TTF at a **relative** path (see F4) — e.g. `MGContent/Fonts/NotoSansSC-Regular.ttf` — as a normal `Content`/`None`+CopyToOutput asset (NOT a `.spritefont`, so the pipeline doesn't bake an atlas).
- Introduce a thin `IFont`/`FontService` abstraction wrapping `FontSystem` so existing call
  sites that use `GraphicsManager.Font` (a `SpriteFont`) keep working. Provide
  `MeasureString`/`DrawString` shims with the same signatures used today.
- Glyphs rasterize on demand and cache at runtime → no giant atlas, any Unicode renders.

**Touch points (verify before changing):**
- `Client.Main/Controllers/GraphicsManager.cs` — `Font` property + `LoadFontWithFallback`.
- All `DrawString`/`MeasureString` callers (≈40 files; see `grep -rl "DrawString\|MeasureString" Client.Main`).
- `Constants.FONT_NAME`, `MGContent/Content.mgcb` (remove/replace `CN.spritefont`).

**Acceptance criteria:**
- [ ] Builds on at least MuWinDX **and** MuLinux without a Windows font path.
- [ ] Chinese text entered in chat (arbitrary characters) renders, not just baked ranges.
- [ ] Existing UI (HUD, panels, damage text, nameplates) renders unchanged in size/position.
- [ ] No per-frame font allocations in Draw/Update (cache the `FontSystem`/sizes).
- [ ] Fallback path retained: if the TTF is missing, log and degrade gracefully.

**Review focus:** signature-compat of the shim, perf (glyph cache reuse), DX/GL parity.

---

## P1 — F2: Localized item / map / NPC names

**Why:** Skill names are now curated (PR #1) but items, maps and NPCs still come from the
Korean asset data and render as boxes/`?`.

**Approach:** Mirror the skill pattern. Add zh/en name tables (or extend `lang_*.json`) keyed
by the same IDs the existing databases use, and make the lookups prefer the localized table.

**Touch points:** `Core/Utilities/ItemDatabase.cs`, `MapDatabase.cs`, `NpcDatabase.cs`,
`SkillDatabase.GetSkillName` (reference implementation).

**Acceptance criteria:**
- [ ] `ItemDatabase`/`MapDatabase`/`NpcDatabase` expose a localized `GetName` that prefers
      zh when `Loc.CurrentLanguage == "zh"`, then en, then clean asset data, then `"<type> {id}"`.
- [ ] No mojibake (`�`) or Hangul leaks through for covered IDs.
- [ ] Switching language at runtime updates newly-rendered names.

---

## P2 — F3: Route remaining hardcoded UI strings through `Loc`

**Why:** Today only the character-info stat labels and the skill panel use `Loc.Get`. Most
windows are hardcoded English.

**Approach:** Replace literal UI strings with `Loc.Get(key)` and grow `lang_en.json` /
`lang_zh.json`. Subscribe controls to `Loc.LanguageChanged` to refresh on toggle. Do it per
window to keep PRs reviewable (inventory, shop, trade, vault, quest, pause menu …).

**Acceptance criteria:**
- [ ] Every newly-keyed string exists in **both** `lang_en.json` and `lang_zh.json`.
- [ ] No key is referenced without a value (so `Loc.Get` never falls back to the raw key in zh).
- [ ] Language toggle in the pause menu visibly re-localizes open windows.

---

## P3 — F4: Bundle an OSS CJK font at a relative path

**Why:** Remove the `C:\Windows\Fonts\msyh.ttc` absolute path so all heads + CI build.
**Note:** Required by F1; can land together. Use a permissively-licensed font (SIL OFL, e.g.
Noto Sans SC). Keep it under `MGContent/Fonts/` like the existing `NotoSansKR-Regular.ttf`.

**Acceptance criteria:**
- [ ] No absolute/OS-specific font path remains in `MGContent/*`.
- [ ] License file/attribution included for the bundled font.

---

## P4 — F5: Server-side message localization (optional)

Chat/system/drop messages are sent by OpenMU in English. If Chinese is wanted there, localize
the server message resources. This is a **server** change, tracked here only for visibility.

---

## P5 — F6: Persist quick-slot bindings + on-screen key hints

**Why:** Q/W/E/A/S/1-3 skill/potion bindings reset each session and are undiscoverable.

**Acceptance criteria:**
- [ ] Bindings persist per character (save/restore).
- [ ] Small key-letter hint drawn on each quick slot.

---

## P0 — F7: Some skills don't cast on right-click (only the active one works)

**Symptom (reported):** only the auto-selected skill casts; after selecting another (e.g.
Hellfire) right-click does nothing. *Names fixed separately in PR #3; this is cast behavior.*

**Findings:**
- `skill.bmd` is positional (record index == skill number), so `GetSkillDefinition(id)` is
  fine; the controller's hardcoded IDs (Teleport=6, Twister=8, Hellfire=10, Inferno=14,
  EvilSpirit=9) match `SkillDefinitions`.
- Likely real causes, all currently **silent**:
  - **Target-type** skills need a monster directly under the cursor — `GetHoveredSkillTarget`
    returns null otherwise and nothing is sent. **Area** skills (Hellfire/Inferno/EvilSpirit)
    cast without a target.
  - `TryBeginSkillCast` returns false with no player feedback on insufficient **mana/AG**,
    active cooldown (`TryConsumeSkillDelay`), or **SafeZone**.

**Actions:**
- Set `GameSceneSkillController`'s logger to Debug and capture which guard fires for a
  failing skill; confirm `CurrentMana`/`CurrentAbility` are synced from the server.
- Add **user-facing feedback** (chat line + error SFX) whenever a cast is rejected, so it is
  never silent (mana/AG/cooldown/SafeZone/no-target).

**Acceptance criteria:**
- [ ] Every learned skill the player selects either casts or shows a clear on-screen reason.
- [ ] Area skills fire without a target; target skills give feedback when nothing is targeted.
- [ ] No silent right-click no-ops.

---

## P0 — F8: Chinese / IME text input on desktop

**Root cause (confirmed):** `Controls/UI/TextFieldControl.cs` on Windows/Desktop polls
`Keyboard.GetPressedKeys()` and maps via `KeyToChar` (ASCII only). It never subscribes to
MonoGame's `GameWindow.TextInput` event (only the Android branch uses a text-input event), so
IME-composed CJK characters never reach the field. Typing Chinese is impossible by design.

**Approach:**
- On focus (desktop), subscribe to `MuGame.Instance.Window.TextInput`; append `e.Character`
  (delivers IME-committed Unicode on WindowsDX/DesktopGL). Unsubscribe on blur. Keep
  Back/Enter handling. Stop using `KeyToChar` for text content on desktop.
- Pair with **F1** so composed glyphs actually render.

**Acceptance criteria:**
- [ ] With a Chinese IME active, typing in chat/name fields inserts Chinese characters.
- [ ] Backspace / Enter still work; pure-ASCII typing is unaffected.
- [ ] Works on both MuWinDX and MuWinGL.

---

### Working agreement
- Implementing agent: take items top-down, **one PR per item**, reference the item ID
  (F1…F8) in the PR title, and fill the acceptance checklist in the PR body.
- Reviewer (me): review each PR against the acceptance criteria above and the build/run
  checklist in `GAME_REVIEW.md` §6, then request changes or approve.
