namespace Utilities {
	using Sirenix.OdinInspector;
	using UI.CharacterCreation;
	using UnityEngine;

	[HideReferenceObjectPicker, DrawChildElements]
	public class AbstractObjectMethodCaller {
		[Tooltip("The name of the object that will be searched in the scene.")]
		public string ObjectName;

		[Tooltip("The component of which the method will be called.")]
		[OnValueChanged("Reset"),ValueDropdown("GetFilteredTypeList")] public System.Type ObjectComponent;

		[Tooltip("The method that will be called")]
		[ValueDropdown("GetMethods")] public string MethodToCall;

		public void Invoke() {
			var go = GameObject.Find(ObjectName);
			if (!go) { Debug.LogError($"Object {ObjectName} not found in scene."); }

			var component = go.GetComponent(ObjectComponent);
			if (!component) { Debug.LogError($"Object {ObjectName} does not contain a component of type {ObjectComponent.Name}."); }

			System.Reflection.BindingFlags _bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
			var method = ObjectComponent.GetMethod(MethodToCall, _bindingFlags);
			if (method == null) { Debug.LogError($"Object {ObjectName}'s component of type {ObjectComponent.Name} does not contain a method called {MethodToCall}()."); }

			method.Invoke(component, null);
		}

		void Reset() => MethodToCall = "";
		ValueDropdownList<string> GetMethods() {

			ValueDropdownList<string> list = new ValueDropdownList<string>();
			if (ObjectComponent == null) { return list; }

			System.Reflection.BindingFlags _bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
			var methods = ObjectComponent.GetMethods(_bindingFlags);
			foreach (var method in methods) {
				if (method.ReturnType != typeof(void)) { continue; }
				if (method.GetParameters().Length != 0) { continue; }
				if (method.IsGenericMethod) { continue; }
				if (method.Name == "StopAllCoroutines") { continue; }
				if (method.Name == "CancelInvoke") { continue; }

				list.Add(method.Name);
			}

			return list;
		}

		public ValueDropdownList<System.Type> GetFilteredTypeList() {
			var list = new ValueDropdownList<System.Type>();

			var types = typeof(Objects.UseableObject).Assembly.GetTypes();
			foreach (var type in types) {
				if (type.IsAbstract) { continue; }
				if (type.IsGenericTypeDefinition) { continue; }
				if (type.ContainsGenericParameters) { continue; }
				if (!typeof(MonoBehaviour).IsAssignableFrom(type)) { continue; }
				list.Add(type.Name, type);
			}

			return list;
		}

	}
}