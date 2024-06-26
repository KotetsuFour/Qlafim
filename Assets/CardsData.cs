using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public class CardsData
{
    public bool[] isHuman;
    public int[] attributes;
    public string[] cardNames;
    public string[] abilityDescriptions;
    public string[] cardIDs;
    public int[] rarities;
    public int[] numDuplicatesAlloweds;
    public byte[][] faceImages;

    public int[][] humanTypes;
    public int[] energyCosts;
    public int[] atks;
    public int[] defs;

    public int[][] allowTriggers;
    public string[][] allowCodes;
    public int[][] reactTriggers;
    public string[][] reactCodes;

    public int[] versions;

    public CardsData(List<TradingCard> cards)
    {
        initialize(cards.Count);

        for (int q = 0; q < cards.Count; q++)
        {
            TradingCard card = cards[q];

            attributes[q] = (int)card.attribute;
            cardNames[q] = card.cardName;
            abilityDescriptions[q] = card.abilityDescription;
            cardIDs[q] = card.cardID;
            rarities[q] = card.rarity;
            numDuplicatesAlloweds[q] = card.numDuplicatesAllowed;
            faceImages[q] = ImageConversion.EncodeToPNG(card.getFaceImage().texture);

            allowTriggers[q] = card.allowTriggers;
            allowCodes[q] = card.allowCode;
            reactTriggers[q] = card.reactTriggers;
            reactCodes[q] = card.reactCode;

            versions[q] = card.version;

            if (card is Human)
            {
                isHuman[q] = true;
                Human human = (Human)card;
                humanTypes[q] = new int[human.humanTypes.Count];
                for (int w = 0; w < human.humanTypes.Count; w++)
                {
                    humanTypes[q][w] = (int)human.humanTypes[w];
                }
                energyCosts[q] = human.energyCost.value;
                atks[q] = human.atk.value;
                defs[q] = human.def.value;
            }
            else if (card is NonHuman)
            {
                isHuman[q] = false;
                humanTypes[q] = new int[1];
                energyCosts[q] = -1;
                atks[q] = -1;
                defs[q] = -1;
            }
        }
    }

    public CardsData()
    {

    }

    public void initialize(int amount)
    {
        isHuman = new bool[amount];
        attributes = new int[amount];
        cardNames = new string[amount];
        abilityDescriptions = new string[amount];
        cardIDs = new string[amount];
        rarities = new int[amount];
        numDuplicatesAlloweds = new int[amount];
        faceImages = new byte[amount][];

        humanTypes = new int[amount][];
        energyCosts = new int[amount];
        atks = new int[amount];
        defs = new int[amount];

        allowTriggers = new int[amount][];
        allowCodes = new string[amount][];
        reactTriggers = new int[amount][];
        reactCodes = new string[amount][];

        versions = new int[amount];
    }

    public TradingCard getCard(int idx, Human hPrefab, NonHuman nhPrefab)
    {
        TradingCard ret;
        if (isHuman[idx])
        {
            ret = Object.Instantiate(hPrefab);
        }
        else
        {
            ret = Object.Instantiate(nhPrefab);
        }

        ret.attribute = (TradingCard.Attribute)attributes[idx];
        ret.cardName = cardNames[idx];
        ret.abilityDescription = abilityDescriptions[idx];
        ret.cardID = cardIDs[idx];
        ret.rarity = rarities[idx];
        ret.numDuplicatesAllowed = numDuplicatesAlloweds[idx];
        ret.setFaceImage(faceImages[idx]);

        ret.allowTriggers = allowTriggers[idx];
        ret.allowCode = allowCodes[idx];
        ret.reactTriggers = reactTriggers[idx];
        ret.reactCode = reactCodes[idx];

        ret.version = versions[idx];

        if (ret is Human)
        {
            Human human = (Human)ret;
            human.humanTypes = new List<Human.HumanType>();
            foreach (Human.HumanType type in humanTypes[idx])
            {
                human.humanTypes.Add(type);
            }
            human.energyCost = new IntegerRegister(energyCosts[idx]);
            human.atk = new IntegerRegister(atks[idx]);
            human.def = new IntegerRegister(defs[idx]);
        }

        return ret;
    }

    public List<TradingCard> getAllCards(Human hPrefab, NonHuman nhPrefab)
    {
        List<TradingCard> ret = new List<TradingCard>();
        for (int q = 0; q < isHuman.Length; q++)
        {
            ret.Add(getCard(q, hPrefab, nhPrefab));
        }
        return ret;
    }

    public void addCards(List<TradingCard> cards)
    {
        if (isHuman == null)
        {
            initialize(cards.Count);
        }

        List<bool> isHumansAdds = new List<bool>(isHuman);
        List<int> attributesAdds = new List<int>(attributes);
        List<string> cardNamesAdds = new List<string>(cardNames);
        List<string> abilityDescriptionsAdds = new List<string>(abilityDescriptions);
        List<string> cardIDsAdds = new List<string>(cardIDs);
        List<int> raritiesAdds = new List<int>(rarities);
        List<int> numDuplicatesAllowedsAdds = new List<int>(numDuplicatesAlloweds);
        List<byte[]> faceImagesAdds = new List<byte[]>(faceImages);

        List<int[]> allowTriggersAdds = new List<int[]>(allowTriggers);
        List<string[]> allowCodesAdds = new List<string[]>(allowCodes);
        List<int[]> reactTriggersAdds = new List<int[]>(reactTriggers);
        List<string[]> reactCodesAdds = new List<string[]>(reactCodes);

        List<int[]> humanTypesAdds = new List<int[]>(humanTypes);
        List<int> energyCostsAdds = new List<int>(energyCosts);
        List<int> atksAdds = new List<int>(atks);
        List<int> defsAdds = new List<int>(defs);

        List<int> versionAdds = new List<int>(versions);

        foreach (TradingCard card in cards)
        {
            attributesAdds.Add((int)card.attribute);
            cardNamesAdds.Add(card.cardName);
            abilityDescriptionsAdds.Add(card.abilityDescription);
            cardIDsAdds.Add(card.cardID);
            raritiesAdds.Add(card.rarity);
            numDuplicatesAllowedsAdds.Add(card.numDuplicatesAllowed);
            faceImagesAdds.Add(ImageConversion.EncodeToPNG(card.getFaceImage().texture));

            allowTriggersAdds.Add(card.allowTriggers);
            allowCodesAdds.Add(card.allowCode);
            reactTriggersAdds.Add(card.reactTriggers);
            reactCodesAdds.Add(card.reactCode);

            versionAdds.Add(card.version);

            if (card is Human)
            {
                isHumansAdds.Add(true);
                Human human = (Human)card;
                humanTypesAdds.Add(new int[human.humanTypes.Count]);
                for (int w = 0; w < human.humanTypes.Count; w++)
                {
                    humanTypesAdds[humanTypesAdds.Count - 1][w] = (int)human.humanTypes[w];
                }
                energyCostsAdds.Add(human.energyCost.value);
                atksAdds.Add(human.atk.value);
                defsAdds.Add(human.def.value);
            }
            else if (card is NonHuman)
            {
                isHumansAdds.Add(false);
                humanTypesAdds.Add(new int[1]);
                energyCostsAdds.Add(-1);
                atksAdds.Add(-1);
                defsAdds.Add(-1);
            }
        }

        isHuman = isHumansAdds.ToArray();
        attributes = attributesAdds.ToArray();
        cardNames = cardNamesAdds.ToArray();
        abilityDescriptions = abilityDescriptionsAdds.ToArray();
        cardIDs = cardIDsAdds.ToArray();
        rarities = raritiesAdds.ToArray();
        numDuplicatesAlloweds = numDuplicatesAllowedsAdds.ToArray();
        faceImages = faceImagesAdds.ToArray();

        allowTriggers = allowTriggersAdds.ToArray();
        allowCodes = allowCodesAdds.ToArray();
        reactTriggers = reactTriggersAdds.ToArray();
        reactCodes = reactCodesAdds.ToArray();

        versions = versionAdds.ToArray();

        humanTypes = humanTypesAdds.ToArray();
        energyCosts = energyCostsAdds.ToArray();
        atks = atksAdds.ToArray();
        defs = defsAdds.ToArray();
    }
}
