using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Azirel.View
{
	public class StringsPairViewControllerPrefab : MonoBehaviour
	{
		[SerializeField] protected Text LeftText;
		[SerializeField] protected Text RightText;

		protected Tuple<string, string> _value;

		public Tuple<string, string> Value
		{
			get => _value;
			set
			{
				_value = value;
				LeftText.text = value.Item1;
				RightText.text = value.Item2;
			}
		}
	}
}