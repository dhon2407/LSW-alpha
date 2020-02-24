// using System;
// using System.Collections;
// using GUI_Animations;
// using LSW.Tooltip;
// using NSubstitute;
// using NUnit.Framework;
// using UnityEditor.VersionControl;
// using UnityEngine.TestTools;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using static UnityEngine.Object;
//
// namespace TooltipTests
// {
//     public class TooltipAreaTest
//     {
//         private const int MagicNumber = 7;
//         private GameObject _gameObj;
//         private TestTooltipArea _tooltipArea;
//         private ITooltip<int> _tooltipStub;
//
//         [SetUp]
//         public void Setup()
//         {
//             _gameObj = Instantiate(new GameObject());
//             _tooltipStub = Substitute.For<ITooltip<int>>();
//
//             _tooltipArea = _gameObj.AddComponent<TestTooltipArea>();
//             _tooltipArea.tooltipData = MagicNumber;
//             _tooltipArea.Init(_tooltipStub);
//         }
//
//         [UnityTest]
//         public IEnumerator Tooltip_Shows_When_MouseEnter()
//         {
//             ExecuteEvent(ExecuteEvents.pointerEnterHandler);
//             yield return null;
//             
//             _tooltipStub.Received().Show();
//             _tooltipStub.Received().SetData(MagicNumber);
//             _tooltipStub.DidNotReceive().Hide();
//         }
//
//         [UnityTest]
//         public IEnumerator Tooltip_Hide_When_MouseExit()
//         {
//             ExecuteEvent(ExecuteEvents.pointerExitHandler);
//             yield return null;
//             
//             _tooltipStub.DidNotReceive().Show();
//             _tooltipStub.DidNotReceive().SetData(Arg.Any<int>());
//             _tooltipStub.Received().Hide();
//         }
//         
//         private void ExecuteEvent<T>(ExecuteEvents.EventFunction<T> eventHandler)
//             where T : IEventSystemHandler
//         {
//             ExecuteEvents.Execute(_gameObj, new PointerEventData(EventSystem.current), eventHandler);
//         }
//
//         private class TestTooltipArea : TooltipArea<int>
//         {
//             public int tooltipData;
//             protected override int TooltipData => tooltipData;
//         }
//     }
// }