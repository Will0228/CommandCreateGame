---
name: push-and-summarize-pr
description: >-
  現在のブランチを GitHub に push し、作者が何をしたかをわかりやすくまとめた
  Pull Request を作成する。ユーザーが push、PR 作成、変更内容のまとめ、
  マージ前の作業振り返りを依頼したときに使う。
---

# Push して作業内容をまとめた PR を作成

ユーザーが **ブランチを push** し、**Pull Request** の本文に **何を・なぜ変更したか** が開発者の言葉で書かれている状態にする。汎用的な AI 文面ではなく、本人の作業報告として読める内容にする。

## 厳守ルール

- ユーザーが草案のタイトルと PR 本文を承認するまで、`git push` と `gh pr create` を実行しない（「確認なしで進めて」と明示された場合を除く）。
- ユーザーが明示的に依頼しない限り、コミットを作成しない。未コミットの変更がある場合は止め、先にコミットするか PR の対象外にするか確認する。
- ユーザーの明示的な承認なしに、`main` / `master` へ force-push しない。
- git config は変更しない。ユーザーが明示しない限りフックをスキップしない（`--no-verify` 禁止）。
- GitHub の PR 操作は可能な限り **`gh`** を使う。

## 手順

### 1. リポジトリの状態を確認

可能なら並列で実行:

```bash
git status
git branch --show-current
git remote -v
git symbolic-ref refs/remotes/origin/HEAD 2>/dev/null || true
```

**ベースブランチ**を決める（`origin/HEAD` を優先。なければ `master`、次に `main`）。

現在のブランチが push 済みか確認:

```bash
git rev-parse HEAD
git rev-parse @{u} 2>/dev/null || echo "no upstream"
git log --oneline <base>..HEAD
git diff --stat <base>...HEAD
```

ベースより先のコミットが **1 つもない** 場合はユーザーに伝えて終了する。

### 2. 「やったこと」のまとめを作る

次を材料にする:

- `git log <base>..HEAD`（コミットメッセージ・作者）
- `git diff <base>...HEAD`（変更ファイルと内容の性質）
- チャットでユーザーが意図を説明していればそれも反映

まとめは **ユーザーの言語**で書く（日本語で話していれば日本語）。文体の目安:

- 一人称または中立な過去形（「〜を追加した」「〜を修正した」）
- ファイル一覧ではなく **テーマごと**に整理
- コミットや会話から分かる **理由** も書く
- 破壊的変更・マイグレーション・手動確認が必要なら明記

避けること: 「各種改善」などの曖昧な表現、全ファイルの羅列、確認していないのにテスト済みと書くこと。

push の前に **草案** を見せる:

```markdown
## PR 草案

**タイトル:** <1 行。命令形または完了形>

## Summary
- ...

## Test plan
- [ ] ...
```

「この内容で push して PR を作成してよいですか？」と確認する。修正があれば反映する。

### 3. GitHub に push

承認後のみ実行:

```bash
git push -u origin HEAD
```

失敗時（upstream なし、認証エラー、non-fast-forward 拒否など）は短く説明し、安全な対処を提案する。明示承認なしに force-push しない。

### 4. Pull Request を作成

`gh pr create` を使う。本文は次のテンプレートに合わせる:

```markdown
## Summary
<作者がやったこと。箇条書き 2〜5 項目、または短い段落>

## Test plan
<ユーザーがチェックできる項目。Unity 向けの手順があれば含める>
```

**Bash（macOS / Linux / Git Bash）:**

```bash
gh pr create --title "TITLE" --body "$(cat <<'EOF'
## Summary
...

## Test plan
- [ ] ...
EOF
)"
```

**PowerShell（Windows）:**

```powershell
gh pr create --title "TITLE" --body @"
## Summary
...

## Test plan
- [ ] ...
"@
```

マージ先がデフォルト以外のとき:

```bash
gh pr create --base <base-branch> --title "..." --body "..."
```

作成後、**PR の URL** をユーザーに返す。

### 5. 結果を報告

簡潔に:

- push したブランチとリモート名
- PR の URL
- PR に含まれる変更の一行要約

## Unity / このリポジトリ向けメモ

`Assets/`、`Packages/`、シーンに触れる変更のとき:

- テストプラン例: Unity でプロジェクトを開く、Play Mode、該当機能の確認、Console のエラー確認
- Addressables・Input System・URP のアセット変更時は、再ビルドや再インポートが必要か記載

このプロジェクトの既定リモート: `origin` → `Will0228/CommandCreateGame`。ベースブランチは多くの場合 **`master`** だが、必ず `origin/HEAD` で確認し、推測で決めない。

## `gh` がない・未ログインのとき

[GitHub CLI](https://cli.github.com/) のインストールと `gh auth login` を案内し、完了後に再実行する。トークンをチャットに貼らせない。

## 関連スキル

- 1 ブランチの作業を複数 PR に分けたい: **split-to-prs** を使う（このスキルではない）。
