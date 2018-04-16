using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Xsolla {
	 public class ValidatorInputField : MonoBehaviour {
		
		public InputField _input;
		public GameObject error;
		public Text errorText;
		public EventTrigger _eventTrigger;
		public ValidatorFactory.ValidatorType[] types;

		protected int _limit;
		protected bool _isValid = false;
		private bool isActive = false;
		private bool isErrorShowed = false;

		private List<IValidator> validators;
		private string _errorMsg = "error";

		void Awake(){
//			if(_input == null)
//				_input = gameObject.AddComponent<InputField> ();
//			
//			if (_eventTrigger == null)
//				_eventTrigger = _input.gameObject.AddComponent<EventTrigger> ();
			error.gameObject.SetActive (false);
		}

		void Start(){
			_input.onValueChange.AddListener ((s) => Validate (s));
			
			EventTrigger.Entry entryS = new EventTrigger.Entry();
			entryS.eventID =  EventTriggerType.Select;
			entryS.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callback =
				new UnityEngine.Events.UnityAction<BaseEventData>(OnSelected);
			entryS.callback.AddListener (callback);
			
			EventTrigger.Entry entryD = new EventTrigger.Entry();
			entryD.eventID =  EventTriggerType.Deselect;
			entryD.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callback1 =
				new UnityEngine.Events.UnityAction<BaseEventData>(OnDeselected);
			entryD.callback.AddListener (callback1);

			//TODO Problem for unity compability 500 - 510
			_eventTrigger.triggers.Add (entryS);
			_eventTrigger.triggers.Add (entryD);

			if (types != null && validators.Count > 0) {
				foreach (var type in types) {
					validators.Add(ValidatorFactory.GetByType(type));
				}
				SetErrorMsg(validators[0].GetErrorMsg());
			}
		}

		public ValidatorInputField(){
			validators = new List<IValidator> ();
		}

		public void AddValidator(IValidator validator){
			validators.Add(validator);
		}

		public void SetErrorMsg(string msg)
		{
			_errorMsg = msg;
		}
		
		private void OnSelected(UnityEngine.EventSystems.BaseEventData baseEvent)
		{
			ShowError (!IsValid());
		}
		
		private void OnDeselected(UnityEngine.EventSystems.BaseEventData baseEvent)
		{
//			if (isActive) {
				ShowError (false);
//			} else {
//				isActive = true;
//				ShowError (!IsValid ());
//			}
		}

		protected void ShowError(bool b){
			if (error != null && errorText != null) { // && isActive
				errorText.text = _errorMsg;
				error.gameObject.SetActive (b);
				isErrorShowed = b;
			}
		}

		
		public bool IsValid() {
			return _isValid;
		}

		public bool Validate(string s){
			foreach (IValidator v in validators) {
				if(!v.Validate(s)) {
					SetErrorMsg(v.GetErrorMsg());
					ShowError(true);
					_isValid = false;
					return false;
				}
			}
			ShowError(false);
			_isValid = true;
			return true;
		}

	}
}
