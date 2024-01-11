using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

namespace Scripts
{
    public class IntroductionTimeline: MonoBehaviour
    {
        public int Index = 0;
        public EventBus EventBus;
        public Scenario Scenario;
        public UIView UIView;
        public List<SmartCharacter> Actors = new List<SmartCharacter>();
        public List<Transform> ActorWayPoints = new List<Transform>();
        //public Transform ActorWayPoint;
        //public Transform ActorSoldierWayPoint;
        //public Transform ActorSoldierTwoWayPoin;
        //public Transform ActorPlayerWayPoint;
        //public Transform ActorEmperorWayPoint;
        //public Transform ActorFirstClerkWayPoint;
        //public Transform ActorSecondClerkWayPoint;
        public Transform CameraWayPoint;
        public Collider Trigger;
        public PlayableDirector PlayableDirector;
        public GameObject Root;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        private int _speedAnimationKey = Animator.StringToHash("Speed");

        public Camera _camera;

        public void Awake()
        {
            _camera = Camera.main;
            _path = new NavMeshPath();
            
            var characters = FindObjectsOfType<SmartCharacter>();
            foreach (var character in characters)
            {
                if (character.CharacterType == Enums.CharacterType.PLAYER)
                {
                    Actors.Add(character);
                }
            }
        }

        public void Update()
        {
            CameraMovement();
        }

        public void FixedUpdate()
        {
            for(int i = 0; i < Actors.Count; i++)
            {
                Actors[i].TargetPosition = ActorWayPoints[i].position;
            }
            //Actors[0].TargetPosition = ActorWayPoint.position;
            //Actors[1].TargetPosition = ActorSoldierWayPoint.position;
            //Actors[2].TargetPosition = ActorPlayerWayPoint.position;
            //Actors[3].TargetPosition = ActorSoldierTwoWayPoin.position;

            foreach (var character in Actors)
            {
                if (character.TargetPosition.sqrMagnitude > float.Epsilon * 2f && character.NavMeshAgent != null)
                {
                    var workerPosition = character.transform.position;

                    var toTargetDelta = character.TargetPosition - workerPosition;
                    toTargetDelta.y = 0f;
                    var toTargetDistance = toTargetDelta.magnitude;

                    if (toTargetDistance < character.FollowingOffset)
                    {
                        character.WorldDirection = Vector3.zero;
                    }
                    else
                    {
                        var pathPosition = character.TargetPosition;

                        if (character.gameObject.activeInHierarchy)
                        {
                            character.NavMeshAgent.enabled = true;
                            if (character.NavMeshAgent.CalculatePath(character.TargetPosition, _path))
                            {
                                var cornersCount = _path.GetCornersNonAlloc(_pathCorners);
                                if (cornersCount > 1)
                                {
                                    pathPosition = _pathCorners[1];
                                }
                                else if (cornersCount > 0)
                                {
                                    pathPosition = _pathCorners[0];
                                }
                            }
                            character.NavMeshAgent.enabled = false;
                        }

                        var pathDelta = pathPosition - workerPosition;
                        // pathDelta.y = 0f;
                        character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

                        if (Physics.Raycast(character.transform.position + Vector3.up * 1.7f, character.WorldDirection, out var hitInfo2, 2f))
                        {
                            if (hitInfo2.collider is CapsuleCollider)
                            {
                                character.WorldDirection = Quaternion.Euler(0f, 45f, 0f) * character.WorldDirection;
                            }
                        }
                    }
                }

                var worldDirection = character.WorldDirection;
                var horizontalDirection = worldDirection;
                horizontalDirection.y = 0f;

                var origin = character.transform.position + Vector3.up * 0.75f;
                var castDirection = horizontalDirection * 0.3f - Vector3.up * 0.75f;
                var hasHitGround = false;
                if (Physics.Raycast(origin, castDirection.normalized, out var hitInfo, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    worldDirection = hitInfo.point - character.transform.position;
                    var worldDirectionY = Mathf.Sign(worldDirection.y) * Mathf.Min(Mathf.Abs(worldDirection.y), 1f);
                    worldDirection = worldDirection.normalized;
                    worldDirection.y = worldDirectionY;
                    hasHitGround = true;
                }

                var characterRigidbody = character.CharacterRigidbody;
                var newCharacterVelocity = worldDirection * character.Speed;
                if (!hasHitGround)
                {
                    newCharacterVelocity.y = characterRigidbody.velocity.y;
                }

                //newCharacterVelocity.y = Mathf.Lerp(characterRigidbody.velocity.y, newCharacterVelocity.y, 0.5f);
                //newCharacterVelocity.y += characterRigidbody.velocity.y;
                characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, newCharacterVelocity, Time.fixedDeltaTime * 8f);
                character.CharacterAnimator.SetFloat(_speedAnimationKey, (character.CharacterRigidbody.velocity.magnitude - 0.5f) / 4f);

                if (horizontalDirection.sqrMagnitude > 0.1f)
                {
                    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.LookRotation(horizontalDirection), Time.fixedDeltaTime * 10f);
                }
            }
        }

        public void ShowEmojiSmile(int characterIndex)
        {
            var character = (Character) Actors[characterIndex];
            EventBus.CallEvent(new ShowEmojiEvent() { Character = character, SpriteIndex = 0 });
        }

        public void ShowPlayerEmoji(int spriteIndex)
        {
            var character = (Character) Actors[3];
            EventBus.CallEvent(new ShowCartoonEvent() { Character = character, SpriteIndex = spriteIndex });
        }

        public void ShowOpponentEmoji(int spriteIndex)
        {
            var character = (Character) Actors[0];
            EventBus.CallEvent(new ShowCartoonEvent() { Character = character, SpriteIndex = spriteIndex });
        }

        public void CameraMovement()
        {
            var newPosition = Vector3.Lerp(_camera.transform.position, CameraWayPoint.position + Scenario.BaseCameraOffset, Time.deltaTime * 5f);
            _camera.transform.position = newPosition;
            _camera.orthographicSize = CameraWayPoint.localScale.x;
        }

        public void PlayAnim(int index)
        {
            var anim = Actors[index].CharacterAnimator;
            anim.SetFloat(_speedAnimationKey, 1f);
        }

        public void StartCutScene() 
        { 
            gameObject.SetActive(true);
            Root.SetActive(true);
            PlayableDirector.Play();
            this.enabled = true;
            Trigger.enabled = false;
        }

        public void Activation()
        {
            Trigger.enabled = true;

            EventBus.CallEvent(new SetArrowPointerEvent() { TargetGameObject = null, TargetPosition = Trigger.bounds.center });
        }

        public void DeactivationCutScene()
        {
            Root.SetActive(false);
            PlayableDirector.Stop();
            Trigger.enabled = false;
            this.enabled = false;
            foreach(var actor in Actors) 
            {
                actor.WorldDirection = Vector3.zero;
            }

            if (EventBus != null)
            {
                EventBus.CallEvent(new SetArrowPointerEvent() { TargetGameObject = null, TargetPosition = Vector3.zero });
            }
        }

        public void IsCatSceneActiv(bool status)
        {
            foreach(var actor in Actors)
            {
                if(actor.CharacterType == Enums.CharacterType.PLAYER)
                {
                    actor.IsCutSceneActiv = status;
                }
            }
        }

        public void InvokeFinal()
        {
            UIView.MenuScreen.gameObject.SetActive(true);
            UIView.SettingsPanel.gameObject.SetActive(false);
            UIView.TitlesPanel.gameObject.SetActive(true);
            DeactivationCutScene();
            Actors[Actors.Count -1].IsCutSceneActiv = false;
        }
    }
}
