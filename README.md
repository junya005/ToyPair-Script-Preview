# ToyPair-Script-Preview

Unity で制作した 2 人協力型アクションゲームのスクリプト群です。プレイヤーの入力・移動・状態管理と、ステージ上の複数のギミックを機能単位で分けて実装しています。

## 構成

| 機能 | コードディレクトリ | 概要 |
| --- | --- | --- |
| Player | [`Assets/Features/Player/Scripts`](Assets/Features/Player/Scripts) | 2 人分の入力、移動、ジャンプ、インタラクション、死亡・復活、表示・音響を管理 |
| Punch | [`Assets/Features/StageGimmick/Scripts/Punch`](Assets/Features/StageGimmick/Scripts/Punch) | パンチで押すスイッチや可動オブジェクトを使ったステージギミック |
| Pythagora | [`Assets/Features/StageGimmick/Scripts/Pythagora`](Assets/Features/StageGimmick/Scripts/Pythagora) | 操作可能なギミック、連動する扉、ゴール、プレイヤーのワープを組み合わせた仕掛け |
| Slope | [`Assets/Features/StageGimmick/Scripts/Slope`](Assets/Features/StageGimmick/Scripts/Slope) | 坂道上のプレイヤーを検出し、向きやダッシュ状態に応じて物理効果を適用 |

## Player

コード: [`Assets/Features/Player/Scripts`](Assets/Features/Player/Scripts)

### 入力と操作

- [`InputManager.cs`](Assets/Features/Player/Scripts/InputManager.cs): プレイヤー番号ごとの入力ハンドルを登録・切り替え・リセットするシングルトン。
- [`PlayerHandle.cs`](Assets/Features/Player/Scripts/PlayerHandle.cs): 移動、ジャンプ、ブレーキ、ダッシュ、インタラクションの入力をデッドゾーン処理し、イベントとして各機能へ通知。
- [`JoyconInput.cs`](Assets/Features/Player/Scripts/JoyconInput.cs): Joy-Con から 2 人分の入力状態を取得。
- [`IPlayerInput.cs`](Assets/Features/Player/Scripts/IPlayerInput.cs)、[`IInputGetter.cs`](Assets/Features/Player/Scripts/IInputGetter.cs): 入力取得側と入力値提供側の契約。
- [`IPlayerHandle.cs`](Assets/Features/Player/Scripts/IPlayerHandle.cs): プレイヤーやギミックが入力ハンドルとして扱われるための契約。
- [`PlayerInputState.cs`](Assets/Features/Player/Scripts/PlayerInputState.cs): 入力状態を表す列挙型。

### プレイヤー制御と物理

- [`PlayerController.cs`](Assets/Features/Player/Scripts/PlayerController.cs): プレイヤーの各コンポーネントを束ね、入力値、移動状態、死亡・復活、アニメーションを統括。
- [`PhysicsLocomotion.cs`](Assets/Features/Player/Scripts/PhysicsLocomotion.cs): Rigidbody を利用した移動、加速度、グリップ、アンダーステアを制御。
- [`PhysicsJump.cs`](Assets/Features/Player/Scripts/PhysicsJump.cs): 物理ベースのジャンプを実行。
- [`ChargeDush.cs`](Assets/Features/Player/Scripts/ChargeDush.cs): チャージからダッシュまでの段階的な加速とキャンセルを処理。
- [`VelocityLimiter.cs`](Assets/Features/Player/Scripts/VelocityLimiter.cs): 最大速度、追加速度、加速処理を管理し、Rigidbody の速度を制限。
- [`VelocityBrake.cs`](Assets/Features/Player/Scripts/VelocityBrake.cs): ブレーキ入力中の減速と速度制限を処理。
- [`SpeedLimit.cs`](Assets/Features/Player/Scripts/SpeedLimit.cs): 基準速度と現在速度を保持し、速度倍率を適用。
- [`CheckGround.cs`](Assets/Features/Player/Scripts/CheckGround.cs): レイキャストで接地状態を確認。
- [`PlayerFrontalWallCollisionDetector.cs`](Assets/Features/Player/Scripts/PlayerFrontalWallCollisionDetector.cs): プレイヤー正面の壁衝突を検出。
- [`PlayerSpin.cs`](Assets/Features/Player/Scripts/PlayerSpin.cs): 指定された対象を回転させる演出を非同期処理で実行。

### 状態、死亡、復活

- [`PlayerDeath.cs`](Assets/Features/Player/Scripts/PlayerDeath.cs): 死亡処理を実行し、プレイヤー番号付きの死亡イベントを発火。
- [`PlayerRespawn.cs`](Assets/Features/Player/Scripts/PlayerRespawn.cs): プレイヤーごとの復活地点を登録し、復活処理を実行。
- [`RespawnPosSetter.cs`](Assets/Features/Player/Scripts/RespawnPosSetter.cs): トリガーに入ったプレイヤーへ復活地点を設定。
- [`DeathTriggerByCollider.cs`](Assets/Features/Player/Scripts/DeathTriggerByCollider.cs): コライダー接触を死亡トリガーとして扱う。
- [`PlayerStan.cs`](Assets/Features/Player/Scripts/PlayerStan.cs): 一定時間プレイヤーを行動不能状態にし、キャンセルにも対応。
- [`PlayerTeleport.cs`](Assets/Features/Player/Scripts/PlayerTeleport.cs): Rigidbody を移動させる非同期テレポート処理。
- [`IDeathable.cs`](Assets/Features/Player/Scripts/IDeathable.cs)、[`IRespawnable.cs`](Assets/Features/Player/Scripts/IRespawnable.cs): 死亡・復活処理の契約。
- [`IGetPlayerStatus.cs`](Assets/Features/Player/Scripts/IGetPlayerStatus.cs): プレイヤー状態を参照するための契約。

### 表示、音、参照管理

- [`PlayerView.cs`](Assets/Features/Player/Scripts/PlayerView.cs): アニメーション、エフェクト、メッシュ、頭部の向き、回転、音をまとめて制御。
- [`PlayerAnimation.cs`](Assets/Features/Player/Scripts/PlayerAnimation.cs): Animator のトリガー、Bool、Float と時間付きアニメーションを操作。
- [`PlayerEffect.cs`](Assets/Features/Player/Scripts/PlayerEffect.cs): ID 付きエフェクトの再生、ループ、停止、表示位置変更を管理。
- [`PlayerSounds.cs`](Assets/Features/Player/Scripts/PlayerSounds.cs): 音種別と AudioClip の対応を管理し、ワンショット・ループ再生を提供。
- [`PlayerSubMesh.cs`](Assets/Features/Player/Scripts/PlayerSubMesh.cs): プレイヤーのサブメッシュや関連オブジェクトの表示・非表示を切り替え。
- [`CharacterHeadView.cs`](Assets/Features/Player/Scripts/CharacterHeadView.cs): 移動速度に応じたキャラクター頭部の向きを計算。
- [`CameraFollow.cs`](Assets/Features/Player/Scripts/CameraFollow.cs): 対象オブジェクトを追従するカメラ位置を制御。
- [`ShadowRendererMover.cs`](Assets/Features/Player/Scripts/ShadowRendererMover.cs): プレイヤーの高さに応じて影用 Renderer の位置・表示を調整。
- [`DisplaySlopeToObject.cs`](Assets/Features/Player/Scripts/DisplaySlopeToObject.cs): プレイヤーの入力方向に応じた坂の向きを対象オブジェクトへ表示。
- [`PlayerReferenceGetter.cs`](Assets/Features/Player/Scripts/PlayerReferenceGetter.cs): PlayerHandle、PlayerController、プレイヤー本体への参照を提供。
- [`PlayerManager.cs`](Assets/Features/Player/Scripts/PlayerManager.cs): 2 人の進行位置、死亡数、復活同期、前後関係、必要に応じた後方プレイヤーのテレポートを管理。
- [`PlayerInteract.cs`](Assets/Features/Player/Scripts/PlayerInteract.cs): トリガー内の [`IInteractable`](Assets/Features/Player/Scripts/IInteractable.cs) を検出して操作を実行。

## Stage Gimmick

### Punch

コード: [`Assets/Features/StageGimmick/Scripts/Punch`](Assets/Features/StageGimmick/Scripts/Punch)

- [`PunchHandler.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchHandler.cs)、[`PunchInteract.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchInteract.cs): プレイヤーがパンチ操作を担当する状態を切り替え。
- [`PunchAction.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchAction.cs): ギミッククリア時に前後の待機を含むパンチ動作を実行。
- [`PunchMoveHandler.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchMoveHandler.cs)、[`PunchMoveInteract.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchMoveInteract.cs): 可動パンチギミックの操作権をプレイヤーへ割り当て。
- [`PunchMove.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchMove.cs): Joy-Con またはデバッグ入力で可動オブジェクトを移動し、移動範囲を制限。
- [`PunchSwitch.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchSwitch.cs): 押下状態を保持し、押下イベントを通知するスイッチ。
- [`PunchSwitchMove.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchSwitchMove.cs): スイッチの押し込み・復帰アニメーションを制御。
- [`PushTrigger.cs`](Assets/Features/StageGimmick/Scripts/Punch/PushTrigger.cs)、[`IPushAble.cs`](Assets/Features/StageGimmick/Scripts/Punch/IPushAble.cs): 接触した押下可能オブジェクトへ `Push` を伝達する共通機構。
- [`PunchGimmickManager.cs`](Assets/Features/StageGimmick/Scripts/Punch/PunchGimmickManager.cs): 全スイッチの押下を監視し、全て押されたときのクリアイベントを通知。
- [`TestAllSwitchPushed.cs`](Assets/Features/StageGimmick/Scripts/Punch/TestAllSwitchPushed.cs): 全スイッチ押下状態を UI に表示するテスト用スクリプト。

### Pythagora

コード: [`Assets/Features/StageGimmick/Scripts/Pythagora`](Assets/Features/StageGimmick/Scripts/Pythagora)

- [`PythagoraGimmickHandler.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickHandler.cs)、[`PythagoraGimmickInteract.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickInteract.cs): プレイヤーへの操作権を切り替え、ジャンプ・インタラクション入力をギミックへ中継。
- [`PythagoraGimmickController.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickController.cs): 入力に応じて複数の扉を順番に切り替えるギミック制御。
- [`PythagoraGimmickDoor.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickDoor.cs): 扉の開閉状態、回転アニメーション、コライダーを制御。
- [`PythagoraGimmickGoal.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickGoal.cs): プレイヤー到達時に復活処理と UnityEvent のゴール処理を実行。
- [`GimmickWarpPlayer.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/GimmickWarpPlayer.cs): ギミックとの衝突時にプレイヤーを指定地点へワープ。
- [`PythagoraGimmickLimitControl.cs`](Assets/Features/StageGimmick/Scripts/Pythagora/PythagoraGimmickLimitControl.cs): ギミック操作中のプレイヤー入力を制限し、坂面に沿った力を適用。

### Slope

コード: [`Assets/Features/StageGimmick/Scripts/Slope`](Assets/Features/StageGimmick/Scripts/Slope)

- [`SlopePlayerChecker.cs`](Assets/Features/StageGimmick/Scripts/Slope/SlopePlayerChecker.cs): 坂に接触しているプレイヤーを番号付きで追跡し、退出イベントを通知。
- [`SlopeConditionsChecker.cs`](Assets/Features/StageGimmick/Scripts/Slope/SlopeConditionsChecker.cs): プレイヤーの移動方向、坂の法線、ダッシュ状態から効果適用条件を判定。
- [`SlopeController.cs`](Assets/Features/StageGimmick/Scripts/Slope/SlopeController.cs): 接触中のプレイヤーを走査し、条件に応じて坂道効果を付与・解除。
- [`SlopeEffectGrantor.cs`](Assets/Features/StageGimmick/Scripts/Slope/SlopeEffectGrantor.cs): 坂の法線に沿う速度補正や追加力をプレイヤーへ適用し、解除時に力をリセット。

## 補助スクリプト

- [`InputDefaultSetter.cs`](Assets/Features/Player/Scripts/InputDefaultSetter.cs): 初期入力ハンドルを設定。
- [`InteractableTest.cs`](Assets/Features/Player/Scripts/InteractableTest.cs): インタラクションの動作確認用サンプル。
- [`TestInputManager.cs`](Assets/Features/Player/Scripts/TestInputManager.cs): プレイヤー、Joy-Con、ピタゴラギミックの入力接続を初期化するテスト用スクリプト。
- [`TestJoyConInputConnector.cs`](Assets/Features/Player/Scripts/TestJoyConInputConnector.cs): Joy-Con の入力を各 PlayerHandle へ転送し、坂道リセット入力を処理。
- [`TestPlayerRotationReset.cs`](Assets/Features/Player/Scripts/TestPlayerRotationReset.cs): プレイヤーの回転リセットを実行するテスト用スクリプト。

## 技術メモ

- Unity の `MonoBehaviour`、物理衝突、Trigger、UnityEvent を中心に構成されています。
- プレイヤーとギミックの操作切り替えは `IPlayerHandle` と `InputManager` を介して行います。
- 時間を伴うアニメーションやエフェクトは Cysharp UniTask とキャンセルトークンを使用しています。
- 外部デバイス入力として Joy-Con を利用し、テスト用のデバッグ入力経路も用意されています。
