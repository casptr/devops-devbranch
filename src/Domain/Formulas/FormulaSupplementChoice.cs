﻿using Ardalis.GuardClauses;
using Domain.Common;
using Domain.QuotationsAndFormulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Formulas
{
	public class FormulaSupplementChoice : Entity
	{
		private string name = default!;
		public string Name { get => name; set => name = Guard.Against.NullOrEmpty(value, nameof(Name)); }

		public bool IsQuantityNumberOfGuests { get; set; }

		private int minQuantity = default!;
		public int MinQuantity { get => minQuantity; set => Guard.Against.Negative(value, nameof(MinQuantity)); }

		private Supplement defaultChoice = default!;
		public Supplement DefaultChoice { get => defaultChoice; set => defaultChoice = Guard.Against.Null(value, nameof(DefaultChoice)); }

		private readonly List<Supplement> supplementsToChoose = new();
		public IReadOnlyCollection<Supplement> SupplementsToChoose => supplementsToChoose.AsReadOnly();

		/// <summary>
		/// Database Constructor
		/// </summary>
		private FormulaSupplementChoice() { }

		public FormulaSupplementChoice(string name, bool isQuantityNumberOfGuests, Supplement defaultChoice)
		{
			Name = name;
			IsQuantityNumberOfGuests = isQuantityNumberOfGuests;
			DefaultChoice = defaultChoice;
		}

		public void AddSupplementToChoose(Supplement supplement)
		{
			Guard.Against.Null(supplement, nameof(supplement));
			supplementsToChoose.Add(supplement);
		}

	}
}
