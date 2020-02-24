// using System.Collections;
// using GUI_Animations;
// using LSW.Tooltip;
// using NSubstitute;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.TestTools;
// using static UnityEngine.Object;
// using Assert = ModestTree.Assert;
//
// namespace TooltipTests
// {
//     public class TooltipTests
//     {
//         private TestToolTip _testToolTip;
//         private GameObject _testObj;
//         private IUiPopup _popupStub;
//
//         [SetUp]
//         public void Setup()
//         {
//             _testObj = Instantiate(new GameObject());
//             _testToolTip = _testObj.AddComponent<TestToolTip>();
//             _popupStub = Substitute.For<IUiPopup>();
//
//             _testToolTip.Init(_popupStub);
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             Destroy(_testObj);
//         }
//         
//         
//         [UnityTest]
//         public IEnumerator Showing()
//         {
//             _testToolTip.Show();
//             yield return null;
//             
//             _popupStub.Received().Show(Arg.Any<UnityAction>());
//             _popupStub.DidNotReceive().Hide(Arg.Any<UnityAction>());
//         }
//         
//         [UnityTest]
//         public IEnumerator Hiding()
//         {
//             _testToolTip.Hide();
//             yield return null;
//
//             _popupStub.DidNotReceive().Show(Arg.Any<UnityAction>());
//             _popupStub.Received().Hide(Arg.Any<UnityAction>());
//         }
//         
//         [UnityTest]
//         public IEnumerator Follow_MousePosition_When_Shown()
//         {
//             _testToolTip.Show();
//             yield return null;
//
//             _popupStub.Received().Show(Arg.Any<UnityAction>());
//
//             const int testIteration = 10;
//             for (int i = 0; i < testIteration; i++)
//             {
//                 yield return null;
//                 Assert.IsEqual(Vector2.one * i, (Vector2)_testToolTip.transform.position);
//                 _testToolTip.testPosition += Vector2.one;
//             }
//         }
//         
//         [UnityTest]
//         public IEnumerator Stop_Follow_MousePosition_When_Hidden()
//         {
//             _testToolTip.Hide();
//             yield return null;
//
//             _popupStub.Received().Hide(Arg.Any<UnityAction>());
//
//             const int testIteration = 10;
//             for (int i = 0; i < testIteration; i++)
//             {
//                 yield return null;
//                 Assert.IsEqual(Vector2.zero, (Vector2)_testToolTip.transform.position);
//                 _testToolTip.testPosition += Vector2.one;
//             }
//         }
//         
//         private class TestToolTip : Tooltip<int>
//         {
//             public Vector2 testPosition = Vector2.zero;
//             public override void SetData(int data) {}
//             protected override Vector2 MousePosition => testPosition;
//         }
//     }
// }
