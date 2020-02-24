using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CharacterStats;
using GUI_Animations;
using Stats = PlayerStats.Stats;
using Objects.Functionality;
using Objects;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using GameTime;

public class Bank : SerializedMonoBehaviour, IInteractable
{
	public float MoneyInBank;
	public float PercentagePerDay = 0.01f;
	public static Bank Instance;

	[SerializeField] private BankUI bankUI = null;

	public InteractionOptions interactionOptions => _interactionOptions;
	[OdinSerialize, ShowInInspector] InteractionOptions _interactionOptions = null;

	public bool isValidInteractionTarget => true;
	public float CurrentBalance => MoneyInBank;

	public void Interact() => ToggleBankUI();

	void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			bankUI.Initialize(this);
		}
	}
    private void Start()
    {
        Clock.onDayPassed.AddListener(AddPercentageToBalance);
    }

    void Update()
	{
		if (bankUI.Visible && Vector2.Distance(GameLibOfMethods.player.transform.position, transform.position) > 1.5f)
			HideBankUI();
	}

	public bool Deposit(float amount)
	{
		if (amount > Stats.Money)
		{
			GameLibOfMethods.CreateFloatingText("Not enough money!", 3);
			return false;
		}

		Stats.RemoveMoney(amount);
		MoneyInBank += amount;
		UpdateBalance();
		
		return true;
	}

	public bool Withdraw(float amount)
	{
		if (amount > MoneyInBank) return false;
		
		Stats.AddMoney(amount);
		MoneyInBank -= amount;
		UpdateBalance();

		return true;
	}

	public void AddPercentageToBalance()
	{
		MoneyInBank += MoneyInBank * PercentagePerDay;
		UpdateBalance();
	}

	public void UpdateBalance()
	{
		bankUI.UpdateBalance();
	}

	private void ShowBankUI()
	{
		bankUI.Open();
	}

	private void HideBankUI()
	{
		bankUI.Close();
	}

	private void ToggleBankUI()
	{
		if (bankUI.Visible)
			HideBankUI();
		else
			ShowBankUI();
	}
}
