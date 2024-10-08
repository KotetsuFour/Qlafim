using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNotification
{

    public Nature nature;
    public bool disputable;
    public NotificationHandler cause;
    public List<IntegerRegister> integerRegistry;
    public List<TradingCard> cardRegistry;
    public List<PositionState> positionStateRegistry;

    private List<PermissionDenial> permissionsQueue;
    private PermissionDenial currentPermission;
    private Stage currentStage;
    private bool denied;

    public static int NOTIFICATION_TYPES = 15;


    public GameNotification(Nature nature, bool disputable, NotificationHandler cause)
    {
        this.nature = nature;
        this.disputable = disputable;
        this.cause = cause;
        this.currentStage = Stage.QUEUED;

        integerRegistry = new List<IntegerRegister>();
        cardRegistry = new List<TradingCard>();
        positionStateRegistry = new List<PositionState>();
        permissionsQueue = new List<PermissionDenial>();
    }

    public void deny(NotificationHandler handler)
    {
        if (currentPermission == null)
        {
            permissionsQueue.Add(new PermissionDenial(handler, false, this));
        }
        else
        {
            permissionsQueue.Add(new PermissionDenial(handler, false, currentPermission));
        }
    }

    public enum Nature
    {
        /*0*/GAME_STATE,
        /*1*/PLAYER_ACTION, //0 for playCard (second int is for position),
                            //1 for reposition (0 is face-down, 1 is face-up defense, 2 is attack),
                            //2 for attack (first card is attacker, second card is defender)
        /*2*/PERM_ALTER_ATK,
        /*3*/PERM_ALTER_DEF,
        /*4*/TEMP_ALTER_ATK,
        /*5*/TEMP_ALTER_DEF,
        /*6*/PERM_ALTER_COST,
        /*7*/TEMP_ALTER_COST,
        /*8*/ALTER_LIFE_PTS,
        /*9*/MOVE_CARD,
        /*10*/ACTIVATE_ABILITY,
        /*11*/PERM_SET_COST,
        /*12*/TEMP_SET_COST,
        /*13*/CHANGE_ENERGY,
        /*14*/SET_ATTRIBUTE
    }
    public enum Stage
    {
        QUEUED, ACTING, COMPLETED, DENIED
    }
    public bool isDone()
    {
        if (currentStage == Stage.QUEUED)
        {
            resolve(); //Sets current stage to ACTING or DENIED
            return false;
        }
        if (currentStage == Stage.ACTING)
        {
            act();
            return false;
        }
        if (currentStage == Stage.COMPLETED || currentStage == Stage.DENIED)
        {
            return true;
        }
        return false;
    }
    private void act()
    {
        if (nature == Nature.GAME_STATE)
        {
            bool turn = StaticData.board.isMyTurn();
            if (StaticData.board.getCurrentPhase() == Gameboard.Phase.MAIN2)
            {
                StaticData.board.changePhaseNotificationCall(!turn, Gameboard.Phase.DRAW);
            } else
            {
                StaticData.board.changePhaseNotificationCall(turn, StaticData.board.getCurrentPhase() + 1);
            }
        }
        else if (nature == Nature.PLAYER_ACTION)
        {
            if (integerRegistry[0].value == 0)
            {
                if (StaticData.board.isMyTurn())
                {
                    StaticData.board.myPositionStates[1].remove(cardRegistry[0]);
                    if (cardRegistry[0] is Human)
                    {
                        StaticData.board.myPositionStates[2].addCard(cardRegistry[0]);
                    }
                    else if (cardRegistry[0] is NonHuman)
                    {
                        StaticData.board.myPositionStates[3].addCard(cardRegistry[0]);
                    }
                    GameObject.Find("NetworkLogic").GetComponent<NetworkLogic>()
                        .setAction(integerRegistry[0].value, integerRegistry[1].value, 0, 0);
                }
                else
                {
                    StaticData.board.yourPositionStates[1].remove(cardRegistry[0]);
                    if (cardRegistry[0] is Human)
                    {
                        StaticData.board.yourPositionStates[2].addCard(cardRegistry[0]);
                    }
                    else if (cardRegistry[0] is NonHuman)
                    {
                        StaticData.board.yourPositionStates[3].addCard(cardRegistry[0]);
                    }
                }
                positionCard(cardRegistry[0], integerRegistry[1].value);

            }
            else if (integerRegistry[0].value == 1
                && (cardRegistry[0].positionState == StaticData.board.myPositionStates[2]
                || cardRegistry[0].positionState == StaticData.board.myPositionStates[3]
                || cardRegistry[0].positionState == StaticData.board.yourPositionStates[2]
                || cardRegistry[0].positionState == StaticData.board.yourPositionStates[3]))
            {
                positionCard(cardRegistry[0], integerRegistry[1].value);

                GameObject.Find("NetworkLogic").GetComponent<NetworkLogic>()
                    .setAction(integerRegistry[0].value, integerRegistry[1].value, 0, 0);
            }
            else if (integerRegistry[0].value == 2)
            {
                //TODO attack animation
                Human atk = (Human)cardRegistry[0];
                Human def = (Human)cardRegistry[1];
                if (def.position == 2)
                {
                    if (atk.atk.effectiveValue() > def.atk.effectiveValue())
                    {
                        GameNotification destroy = new GameNotification(Nature.MOVE_CARD, true, atk);
                        PositionState[] states = StaticData.board.isMyTurn() ? StaticData.board.myPositionStates
                            : StaticData.board.yourPositionStates;
                        destroy.cardRegistry.Add(def);
                        destroy.positionStateRegistry.Add(states[2]);
                        destroy.positionStateRegistry.Add(states[4]);

                        GameNotification damage = new GameNotification(Nature.ALTER_LIFE_PTS, true, atk);
                        damage.integerRegistry.Add(new IntegerRegister(StaticData.board.isMyTurn() ? 1 : 0));
                        damage.integerRegistry.Add(new IntegerRegister(def.atk.effectiveValue() - atk.atk.effectiveValue()));

                        StaticData.board.addNotification(destroy);
                        StaticData.board.addNotification(damage);
                    }
                    else if (atk.atk.effectiveValue() == def.atk.effectiveValue())
                    {
                        GameNotification atkDestroy = new GameNotification(Nature.MOVE_CARD, true, atk);
                        GameNotification defDestroy = new GameNotification(Nature.MOVE_CARD, true, atk);
                        PositionState[] atkStates = StaticData.board.isMyTurn() ? StaticData.board.myPositionStates
                            : StaticData.board.yourPositionStates;
                        PositionState[] defStates = !StaticData.board.isMyTurn() ? StaticData.board.myPositionStates
                            : StaticData.board.yourPositionStates;
                        atkDestroy.cardRegistry.Add(atk);
                        atkDestroy.positionStateRegistry.Add(atkStates[2]);
                        atkDestroy.positionStateRegistry.Add(atkStates[4]);
                        defDestroy.cardRegistry.Add(def);
                        defDestroy.positionStateRegistry.Add(defStates[2]);
                        defDestroy.positionStateRegistry.Add(defStates[4]);

                        StaticData.board.addNotification(atkDestroy);
                        StaticData.board.addNotification(defDestroy);
                    }
                    else if (atk.atk.effectiveValue() < def.atk.effectiveValue())
                    {
                        GameNotification atkDestroy = new GameNotification(Nature.MOVE_CARD, true, atk);
                        PositionState[] atkStates = StaticData.board.isMyTurn() ? StaticData.board.myPositionStates
                            : StaticData.board.yourPositionStates;
                        atkDestroy.cardRegistry.Add(atk);
                        atkDestroy.positionStateRegistry.Add(atkStates[2]);
                        atkDestroy.positionStateRegistry.Add(atkStates[4]);

                        GameNotification damage = new GameNotification(Nature.ALTER_LIFE_PTS, true, atk);
                        damage.integerRegistry.Add(new IntegerRegister(StaticData.board.isMyTurn() ? 0 : 1));
                        damage.integerRegistry.Add(new IntegerRegister(atk.atk.effectiveValue() - def.atk.effectiveValue()));

                        StaticData.board.addNotification(atkDestroy);
                        StaticData.board.addNotification(damage);
                    }
                }
                else
                {
                    if (integerRegistry[0].value == 0)
                    {
                        //TODO flip card and attack animation
                    }
                    else if (integerRegistry[0].value == 1)
                    {
                        //TODO attack animation
                    }

                    if (atk.atk.effectiveValue() > def.def.effectiveValue())
                    {
                        GameNotification destroy = new GameNotification(Nature.MOVE_CARD, true, atk);
                        PositionState[] states = StaticData.board.isMyTurn() ? StaticData.board.myPositionStates
                            : StaticData.board.yourPositionStates;
                        destroy.cardRegistry.Add(def);
                        destroy.positionStateRegistry.Add(states[2]);
                        destroy.positionStateRegistry.Add(states[4]);

                        StaticData.board.addNotification(destroy);
                    }
                    else
                    {
                        GameNotification damage = new GameNotification(Nature.ALTER_LIFE_PTS, true, def);
                        damage.integerRegistry.Add(new IntegerRegister(StaticData.board.isMyTurn() ? 0 : 1));
                        damage.integerRegistry.Add(new IntegerRegister(atk.atk.effectiveValue() - def.def.effectiveValue()));

                        StaticData.board.addNotification(damage);
                    }
                }
                if (StaticData.board.isMyTurn())
                {
                    GameObject.Find("NetworkLogic").GetComponent<NetworkLogic>()
                        .setAction(integerRegistry[0].value, 0, 
                        StaticData.board.myPositionStates[2].cardsHere.IndexOf(cardRegistry[0]),
                        StaticData.board.yourPositionStates[2].cardsHere.IndexOf(cardRegistry[1]));
                }
            }
        }
        else if (nature == Nature.PERM_ALTER_ATK)
        {
            ((Human)cardRegistry[0]).atk.changePermanently(integerRegistry[0].value);
        }
        else if (nature == Nature.PERM_ALTER_DEF)
        {
            ((Human)cardRegistry[0]).def.changePermanently(integerRegistry[0].value);
        }
        else if (nature == Nature.TEMP_ALTER_ATK)
        {
            ((Human)cardRegistry[0]).atk.changeTemporarily(integerRegistry[0].value);
        }
        else if (nature == Nature.TEMP_ALTER_DEF)
        {
            ((Human)cardRegistry[0]).def.changeTemporarily(integerRegistry[0].value);
        }
        else if (nature == Nature.PERM_ALTER_COST)
        {
            ((Human)cardRegistry[0]).energyCost.changePermanently(integerRegistry[0].value);
        }
        else if (nature == Nature.TEMP_ALTER_COST)
        {
            ((Human)cardRegistry[0]).energyCost.changeTemporarily(integerRegistry[0].value);
        }
        else if (nature == Nature.ALTER_LIFE_PTS)
        {
            StaticData.board.alterLifePoints(integerRegistry[0].value == 0, integerRegistry[1].value);
        }
        else if (nature ==  Nature.MOVE_CARD)
        {
            if (positionStateRegistry[0].cardsHere.Contains(cardRegistry[0])
                && !positionStateRegistry[1].isFull())
            {
                //TODO animation
                positionStateRegistry[0].remove(cardRegistry[0]);
                positionStateRegistry[1].addCard(cardRegistry[0]);
                cardRegistry[0].position = 0;
                Debug.Log("Move card resolved");
            }
            Debug.Log("Move card tried to resolve");
        }
        else if (nature == Nature.ACTIVATE_ABILITY)
        {
            //Nothing
        }
        else if (nature == Nature.PERM_SET_COST)
        {
            ((Human)cardRegistry[0]).energyCost.value = integerRegistry[0].value;
        }
        else if (nature == Nature.TEMP_SET_COST)
        {
            int val = ((Human)cardRegistry[0]).energyCost.value;
            ((Human)cardRegistry[0]).energyCost.changeTemporarily(integerRegistry[0].value - val);
        }
        else if (nature == Nature.CHANGE_ENERGY)
        {
            StaticData.board.alterEnergy(integerRegistry[0].value == 0, integerRegistry[1].value);
        }
        else if (nature == Nature.SET_ATTRIBUTE)
        {
            TradingCard.Attribute att = (TradingCard.Attribute)integerRegistry[0].value;
            cardRegistry[0].attribute = att;
            //TODO set UI representation
        }

        currentStage = Stage.COMPLETED;
    }
    private void positionCard(TradingCard card, int position)
    {
        integerRegistry.Add(new IntegerRegister(card.position));
        if (card is Human)
        {
            if (position == 0)
            {
                card.transform.rotation = Quaternion.Euler(0, 90, 180);
            }
            else if (position == 1)
            {
                card.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (position == 2)
            {
                card.transform.rotation = Quaternion.identity;
            }
        }
        else if (card is NonHuman)
        {
            if (position == 0)
            {
                card.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                card.transform.rotation = Quaternion.identity;
            }
        }
    }
    public bool wasDenied()
    {
        return currentStage == Stage.DENIED;
    }
    private void resolve()
    {
        //TODO if NOT disputable, just set to ACTING
        List<TradingCard> actors = StaticData.board.getAllCardsInGame();
        for (int q = 0; q < actors.Count; q++)
        {
            actors[q].allowNotification(this);
        }
        for (int q = 0; q < permissionsQueue.Count; q++)
        {
            for (int w = 0; w < actors.Count; w++)
            {
                actors[w].allowNotification(this);
            }
        }
        for (int q = permissionsQueue.Count - 1; q >= 0; q--)
        {
            permissionsQueue[q].deny();
        }
        if (denied)
        {
            currentStage = Stage.DENIED;
        }
        else
        {
            currentStage = Stage.ACTING;
        }
    }

    public class PermissionDenial
    {
        public NotificationHandler actor;
        public bool permitted;
        public GameNotification notificationSubject;
        public PermissionDenial permissionSubject;

        public bool denied;
        public PermissionDenial(NotificationHandler actor, bool permitted, PermissionDenial permissionSubject)
        {
            this.actor = actor;
            this.permitted = permitted;
            this.permissionSubject = permissionSubject;
        }
        public PermissionDenial(NotificationHandler actor, bool permitted, GameNotification notificationSubject)
        {
            this.actor = actor;
            this.permitted = permitted;
            this.notificationSubject = notificationSubject;
        }
        public void deny()
        {
            if (!denied && !permitted)
            {
                if (permissionSubject == null)
                {
                    notificationSubject.denied = true;
                }
                else
                {
                    permissionSubject.denied = true;
                }
            }
        }
    }

}
