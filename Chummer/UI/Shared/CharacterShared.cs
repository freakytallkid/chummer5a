/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Runtime.InteropServices;
﻿using System.Windows.Forms;

namespace Chummer
{
	/// <summary>
	/// Contains functionality shared between frmCreate and frmCareer
	/// </summary>
	[System.ComponentModel.DesignerCategory("")]
	public class CharacterShared : Form
	{
		protected Character _objCharacter;
		protected MainController _objController;
		protected CharacterOptions _objOptions;
		protected CommonFunctions _objFunctions;

		/// <summary>
		/// Wrapper for relocating contact forms. 
		/// </summary>
		public class TransportWrapper
		{
			private Control control;

			public TransportWrapper(Control control)
			{
				this.control = control;
			}

			public Control Control
			{
				get { return control; }
			}
		}

		protected void RedlinerCheck()
		{

		    string strSeekerImprovPrefix = "SEEKER";
            //Get attributes affected by redliner/cyber singularity seeker
			var attributes = new List<string>(
				from improvement in _objCharacter.Improvements
				where improvement.ImproveType == Improvement.ImprovementType.Seeker
				select improvement.ImprovedName);

			//And the improvements comming from there
			var impr = new List<Improvement>(
				from improvement in _objCharacter.Improvements
				where (improvement.ImproveType == Improvement.ImprovementType.Attribute ||
				       improvement.ImproveType == Improvement.ImprovementType.PhysicalCM )&&
				       improvement.SourceName.Contains(strSeekerImprovPrefix) //for backwards compability
				select improvement);

			//if neither contains anything, it is safe to exit
		    if (impr.Count == 0 && attributes.Count == 0)
		    {
		        _objCharacter.RedlinerBonus = 0;
                return;
		    }

			//Calculate bonus from cyberlimbs
			int count = Math.Min(_objCharacter.Cyberware.Count(c => c.LimbSlot != "" && c.Name.Contains("Full")) / 2,2);
		    if (impr.Any(x => x.ImprovedName == "STR" || x.ImprovedName == "AGI"))
		    {
		        _objCharacter.RedlinerBonus = count;
		    }
		    else
		    {
                _objCharacter.RedlinerBonus = 0;
            }

		    for (int i = 0; i < attributes.Count; i++)
		    {
		        Improvement objImprove = impr.FirstOrDefault(x => x.SourceName == strSeekerImprovPrefix +"_" + attributes[i] && x.Value == (attributes[i] == "BOX" ? count * -3 : count));
		        if (objImprove != null)
		        {
		            attributes.RemoveAt(i);
		            impr.Remove(objImprove);
		        }
		    }			
			//Improvement manager defines the functions we need to manipulate improvements
			//When the locals (someday) gets moved to this class, this can be removed and use
			//the local
			Lazy<ImprovementManager> manager = new Lazy<ImprovementManager>(() => new ImprovementManager(_objCharacter));

            // Remove which qualites have been removed or which values have changed
            foreach (Improvement improvement in impr)
            {
                manager.Value.RemoveImprovements(improvement.ImproveSource, improvement.SourceName);
            }

            // Add new improvements or old improvements with new values
            foreach (string attribute in attributes)
			{
			    if (attribute == "BOX")
			    {
                    manager.Value.CreateImprovement(attribute, Improvement.ImprovementSource.Quality,
			            strSeekerImprovPrefix + "_" + attribute, Improvement.ImprovementType.PhysicalCM,
			            Guid.NewGuid().ToString(), count*-3);
			    }
			    else
			    {
			        manager.Value.CreateImprovement(attribute, Improvement.ImprovementSource.Quality,
			            strSeekerImprovPrefix +"_" + attribute, Improvement.ImprovementType.Attribute,
			            Guid.NewGuid().ToString(), count, 1, 0, 0, count);
			    }
			}
            if (manager.IsValueCreated)
			{
				manager.Value.Commit(); //REFACTOR! WHEN MOVING MANAGER, change this to bool
			}
        }

		/// <summary>
		/// Update the label and tooltip for the character's Condition Monitors.
		/// </summary>
		/// <param name="lblPhysical"></param>
		/// <param name="lblStun"></param>
		/// <param name="tipTooltip"></param>
		protected void UpdateConditionMonitor(Label lblPhysical, Label lblStun, ToolTip tipTooltip, ImprovementManager _objImprovementManager)
		{
			// Condition Monitor.
			double dblBOD = _objCharacter.BOD.TotalValue;
			double dblWIL = _objCharacter.WIL.TotalValue;
			int intCMPhysical = _objCharacter.PhysicalCM;
			int intCMStun = _objCharacter.StunCM;

			// Update the Condition Monitor labels.
			lblPhysical.Text = intCMPhysical.ToString();
			lblStun.Text = intCMStun.ToString();
			string strCM = "8 + (BOD/2)(" + ((int)Math.Ceiling(dblBOD / 2)).ToString() + ")";
			if (_objImprovementManager.ValueOf(Improvement.ImprovementType.PhysicalCM) != 0)
				strCM += " + " + LanguageManager.Instance.GetString("Tip_Modifiers") + " (" +
						 _objImprovementManager.ValueOf(Improvement.ImprovementType.PhysicalCM).ToString() + ")";
			tipTooltip.SetToolTip(lblPhysical, strCM);
			strCM = "8 + (WIL/2)(" + ((int)Math.Ceiling(dblWIL / 2)).ToString() + ")";
			if (_objImprovementManager.ValueOf(Improvement.ImprovementType.StunCM) != 0)
				strCM += " + " + LanguageManager.Instance.GetString("Tip_Modifiers") + " (" +
						 _objImprovementManager.ValueOf(Improvement.ImprovementType.StunCM).ToString() + ")";
			tipTooltip.SetToolTip(lblStun, strCM);
		}

		/// <summary>
		/// Update the label and tooltip for the character's Armor Rating.
		/// </summary>
		/// <param name="lblArmor"></param>
		/// <param name="tipTooltip"></param>
		protected void UpdateArmorRating(Label lblArmor, ToolTip tipTooltip, ImprovementManager _objImprovementManager)
		{
			// Armor Ratings.
			lblArmor.Text = _objCharacter.TotalArmorRating.ToString();
			string strArmorToolTip = "";
			strArmorToolTip = LanguageManager.Instance.GetString("Tip_Armor") + " (" + _objCharacter.ArmorRating.ToString() + ")";
			if (_objCharacter.ArmorRating != _objCharacter.TotalArmorRating)
				strArmorToolTip += " + " + LanguageManager.Instance.GetString("Tip_Modifiers") + " (" +
								   (_objCharacter.TotalArmorRating - _objCharacter.ArmorRating).ToString() + ")";
			tipTooltip.SetToolTip(lblArmor, strArmorToolTip);

			// Remove any Improvements from Armor Encumbrance.
			_objImprovementManager.RemoveImprovements(Improvement.ImprovementSource.ArmorEncumbrance, "Armor Encumbrance");
			// Create the Armor Encumbrance Improvements.
			if (_objCharacter.ArmorEncumbrance < 0)
			{
				_objImprovementManager.CreateImprovement("AGI", Improvement.ImprovementSource.ArmorEncumbrance, "Armor Encumbrance", Improvement.ImprovementType.Attribute, "", 0, 1, 0, 0, _objCharacter.ArmorEncumbrance);
				_objImprovementManager.CreateImprovement("REA", Improvement.ImprovementSource.ArmorEncumbrance, "Armor Encumbrance", Improvement.ImprovementType.Attribute, "", 0, 1, 0, 0, _objCharacter.ArmorEncumbrance);
			}
		}



		/// <summary>
		/// Update the labels and tooltips for the character's Attributes.
		/// </summary>
		/// <param name="objAttribute"></param>
		/// <param name="lblATTMetatype"></param>
		/// <param name="lblATTAug"></param>
		/// <param name="tipTooltip"></param>
		/// <param name="nudATT"></param>
		protected void UpdateCharacterAttribute(CharacterAttrib objAttribute, Label lblATTMetatype, Label lblATTAug, ToolTip tipTooltip, [Optional] NumericUpDown nudATT)
		{
			if (nudATT != null)
			{
				nudATT.Minimum = objAttribute.TotalMinimum;
				nudATT.Maximum = objAttribute.TotalMaximum;
				nudATT.Value = objAttribute.Value - objAttribute.Karma;
			}
			lblATTMetatype.Text = string.Format("{0} / {1} ({2})", objAttribute.TotalMinimum, objAttribute.TotalMaximum,
				objAttribute.TotalAugmentedMaximum);
			if (objAttribute.HasModifiers)
			{
				lblATTAug.Text = string.Format("{0} ({1})", objAttribute.Value, objAttribute.TotalValue);
				tipTooltip.SetToolTip(lblATTAug, objAttribute.ToolTip());
			}
			else
			{
				lblATTAug.Text = string.Format("{0}", objAttribute.Value);
				tipTooltip.SetToolTip(lblATTAug, "");
			}
		}
	}
}
