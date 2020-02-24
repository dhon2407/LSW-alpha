using Cooking.Recipe;
using GUI_Animations;
using InventorySystem;
using LSW.Tooltip;
using PlayerStats;
using PlayerStats.Buffs;
using PlayerStats.Buffs.Core;
using UnityEngine;
using Zenject;

public class TooltipInstaller : MonoInstaller {
	[SerializeField] private GameObject recipeSlotArea = null;
	[SerializeField] private GameObject quickRecipeSlotArea = null;

	public override void InstallBindings() {
		Container.BindFactory<RecipeSlot, RecipeSlot.Factory>().FromComponentInNewPrefab(recipeSlotArea);
		Container.BindFactory<QuickRecipeSlot, QuickRecipeSlot.Factory>().FromComponentInNewPrefab(quickRecipeSlotArea);
		Container.BindFactory<ItemSlot, ItemSlot.Factory>().FromComponentInNewPrefabResource("Inventory/Slot");

		BindSingleton<ITooltip<RecipeTooltip.Data>, RecipeTooltip>();
		BindSingleton<ITooltip<ItemTooltip.Data>, ItemTooltip>();
		BindSingleton<ITooltip<SimpleBigTooltip.Data>, SimpleBigTooltip>();
		BindSingleton<ITooltip<SimpleSmallTooltip.Data>, SimpleSmallTooltip>();
		BindSingleton<ITooltip<StatTooltip.Data>, StatTooltip>();
		BindSingleton<ITooltip<IBuff>, BuffTooltip>();

		Container.Bind<IUiPopup>().To<TooltipPopup>().FromComponentSibling().AsTransient();
	}

	void BindSingleton<T, TU>() where TU : T => Container.Bind<T>().To<TU>().FromComponentsInHierarchy().AsSingle();

}