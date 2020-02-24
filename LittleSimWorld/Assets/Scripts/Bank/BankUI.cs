using System;
using GUI_Animations;
using LSW.Helpers;
using TMPro;
using UI;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentBalance = null;
    [SerializeField] private TMP_InputField inputField = null;

    private RectTransform _window;
    
    public bool Visible => _popup.Visible;

    public void Initialize(Bank bank)
    {
        _bank = bank;
    }
    
    public void Open()
    {
        UpdateBalance();
        inputField.text = string.Empty;
        _popup.Show(null);
    }
    
    public void Close()
    {
        _popup.Hide(null);
    }
    
    private IUiPopup _popup;
    private Bank _bank;


    public void UpdateBalance()
    {
        currentBalance.text = $"£{_bank.CurrentBalance:0}";
    }

    public void Withdraw()
    {
        if (!float.TryParse(inputField.text, out float amount)) return;
        
        if (_bank.Withdraw(amount))
            UpdateBalance();
        else
            TransactionFailed();
    }

    public void Deposit()
    {
        if (!float.TryParse(inputField.text, out float amount)) return;

        if (_bank.Deposit(amount))
            UpdateBalance();
        else
            TransactionFailed();
    }
    private void Awake()
    {
        _popup = GetComponent<IUiPopup>();
        _window = GetComponent<RectTransform>();
    }

    private void Start()
    {
        GamePauseHandler.SubscribeCloseEvent(EscCloseEvent);
    }

    private bool EscCloseEvent()
    {
        if (!_popup.Visible) return false;

        Close();
        return true;
    }

    private void TransactionFailed()
    {
        _window.Shake(0.2f, 5f);
    }
}
