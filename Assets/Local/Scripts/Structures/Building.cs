using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Building: Unit
    {
        public enum ProductionMethod {
            NONE = 0,
            RESOURCE_TO_PRODUCT = 1,
            RESOURCE_TO_TIME = 2,
        }

        [Header("Building Config")]
        public int ID = -1;
        public Collider ConstructionArea;
        public Collider ProductionArea;
        public Collider UnloadingArea;
        public Collider PickingUpArea;
        public Collider UpgradeArea;
        public GameObject ConstructionAreaIndicator;
        public GameObject ProductionAreaIndicator;
        public GameObject UnloadingAreaIndicator;
        public GameObject PickingUpAreaIndicator;
        public GameObject UpgradeAreaIndicator;
        public GameObject ProductionAreaHelper;
        public GameObject PickingUpAreaHelper;
        public Transform ProgressBarPivot;
        public GameObject StopProductionIcon;
        public GameObject NoResourceIcon;
        public ProductionMethod ProduceMethod = ProductionMethod.RESOURCE_TO_PRODUCT;
        public int BaseProductionLimit;
        public int ProductionLimit;
        public int ResourceLimit;
        public int ItemCost;
        public int ProductionItemAmountPerCycle;
        public float ProductionCooldown;
        public bool ProductionUseBagOfTries = true;
        public float ProductionConversionRate = 1f;
        public ItemType ProduceItemType;
        public ItemType ConsumeItemType;
        public List<BuildingLevel> Levels;
        public Unit UpgradeStorage;
        public Animation HonorIconAnimation;
        public AnimationEffect UpgradeAnimationEffect;
        public MeshRenderer ConstructionAreaMeshRenderer;
        public MeshRenderer UpgradeAreaMeshRenderer;
        public MeshRenderer UnloadingAreaMeshRenderer;

        [SerializeField] private int _level;

        [Header("Building Runtime")]
        public List<Character> ConstructionCharacters = new List<Character>();
        public List<Character> ProductionCharacters = new List<Character>();
        public List<Character> UnloadingCharacters = new List<Character>();
        public List<Character> PickingUpCharacters = new List<Character>();
        public List<Character> UpgradeCharacters = new List<Character>();
        public List<Character> AssignedProductionCharacters = new List<Character>();
        public List<Character> AssignedUnloadingCharacters = new List<Character>();
        public List<Character> AssignedPickingUpCharacters = new List<Character>();
        public float LastProductionTime;
        public float ProductionEndTime;
        public float ProductionEndActivityTime;
        public float LastProductionSoundTime;
        public float LastProductionCheckTime;
        public int Level
        {
            get
            {
                return _level;
            }

            set
            {
                if (Initialized && value > _level && UpgradeAnimationEffect != null)
                {
                    UpgradeAnimationEffect.Activate();
                }

                _level = value;
                if (ConstructionArea != null)
                {
                    ConstructionArea.gameObject.SetActive(_level == 0);
                }
                if (ProductionArea != null)
                {
                    ProductionArea.gameObject.SetActive(_level != 0);
                }
                if (UnloadingArea != null)
                {
                    UnloadingArea.gameObject.SetActive(_level != 0);
                }
                if (PickingUpArea != null)
                {
                    PickingUpArea.gameObject.SetActive(_level != 0);
                }
                if (UpgradeArea != null)
                {
                    UpgradeArea.gameObject.SetActive(_level != 0);
                }
                for (var i = 0; i < Levels.Count; i++)
                {
                    Levels[i].Visual.SetActive(i == _level);
                }
            }
        }
        public bool IsWork { get; set; } = true;
        public bool IsWorkAreaIndicatorEnabled { get {
            return ProductionAreaIndicator.activeSelf;
        } set {
            ProductionAreaIndicator.SetActive(value);
        } }

        public int GetLastCustomerHonor()
        {
            return 1;
        }

        public float ProductionProgress()
        {
            if (!IsWork)
            {
                return 0f;
            }

            var currentProductionTime = ProductionEndTime > LastProductionTime ? ProductionEndTime - LastProductionTime : Time.time - LastProductionTime;
            if (currentProductionTime >= ProductionCooldown)
            {
                return 0f;
            }
            return currentProductionTime / ProductionCooldown;
        }
    }
}
