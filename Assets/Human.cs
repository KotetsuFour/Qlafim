using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : TradingCard
{
    public List<HumanType> humanTypes;
    [SerializeField]private int setATK;
    [SerializeField] private int setDEF;
    [SerializeField] private int setNRG;
    public IntegerRegister energyCost;
    public IntegerRegister atk;
    public IntegerRegister def;
    // Start is called before the first frame update
    void Start()
    {
        if (setup)
        {
            atk = new IntegerRegister(setATK);
            def = new IntegerRegister(setDEF);
            energyCost = new IntegerRegister(setNRG);
        }
        atk.setMinValue(0);
        def.setMinValue(0);
        energyCost.setMinValue(1);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO set visual card values
    }

    public enum HumanType
    {
        KING, PROPHET, PRIEST, HIGH_PRIEST, RABBI, DISCIPLE, APOSTLE, 
    }
}
