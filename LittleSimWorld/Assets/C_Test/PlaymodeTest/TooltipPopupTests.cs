// using System.Collections;
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
//     public class TooltipPopupTests
//     {
//         private GameObject _gameObj;
//         private TooltipPopup _popup;
//         private const float Duration = 0.2f;
//         private const float PopInScale = 1f;
//
//         private Vector3 PopupCurrentScale => _popup.transform.localScale;
//
//         [SetUp]
//         public void Setup()
//         {
//             _gameObj = Instantiate(new GameObject());
//             _popup = _gameObj.AddComponent<TooltipPopup>();
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             Destroy(_gameObj);
//         }
//         
//         [UnityTest]
//         public IEnumerator ShouldBe_Hidden_AtStart()
//         {
//             yield return null;
//             
//             Assert.IsEqual(Vector3.zero, _popup.transform.localScale);
//         }
//         
//         [UnityTest]
//         public IEnumerator Shows_After_Duration()
//         {
//             _popup.Show(null);
//             yield return new WaitForSeconds(Duration);
//             
//             Assert.IsEqual(Vector3.one * PopInScale, _popup.transform.localScale);
//         }
//         
//         [UnityTest]
//         public IEnumerator Hides_After_Duration()
//         {
//             yield return Shows_After_Duration();
//             
//             _popup.Hide(null);
//             yield return new WaitForSeconds(Duration);
//             
//             Assert.IsEqual(Vector3.zero, _popup.transform.localScale);
//         }
//         
//         [UnityTest]
//         public IEnumerator Calls_Action_AfterShowing()
//         {
//             var testAction = Substitute.For<UnityAction>();
//             _popup.Show(testAction);
//             yield return new WaitForSeconds(Duration);
//             
//             Assert.IsEqual(Vector3.one * PopInScale, _popup.transform.localScale);
//             
//             yield return new WaitForSeconds(1f);
//             testAction.Received().Invoke();
//         }
//         
//         [UnityTest]
//         public IEnumerator Calls_Action_AfterClosing()
//         {
//             yield return Calls_Action_AfterShowing();
//             
//             var testAction = Substitute.For<UnityAction>();
//             _popup.Hide(testAction);
//             yield return new WaitForSeconds(Duration);
//             
//             Assert.IsEqual(Vector3.zero, _popup.transform.localScale);
//             
//             yield return new WaitForSeconds(1f);
//             testAction.Received().Invoke();
//         }
//         
//         [UnityTest]
//         public IEnumerator Showing_Action_WillNot_Be_Call_When_ClosedImmediately()
//         {
//             var testShowAction = Substitute.For<UnityAction>();
//             var testHideAction = Substitute.For<UnityAction>();
//             _popup.Show(testShowAction);
//             
//             yield return new WaitForSeconds(Duration / 2f);
//             
//             _popup.Hide(testHideAction);
//             Assert.IsNotEqual(Vector3.one * PopInScale, PopupCurrentScale);
//             
//             yield return new WaitForSeconds(Duration);
//             
//             Assert.IsEqual(Vector3.zero, PopupCurrentScale);
//             
//             yield return new WaitForSeconds(1f);
//             testShowAction.DidNotReceive().Invoke();
//             testHideAction.Received().Invoke();
//         }
//     }
// }
