using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    Sprite[] facesRandom = new Sprite[52];
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text probMessage2;
    public Text probMessage3;

    public int[] values = new int[52];
    int[] valuesRandom = new int[52];

    int cardIndex = 0;
  

    private void Awake()
    {    

        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int valorCarta = 11;
        int posicionCarta = 1;

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = valorCarta;

            if (posicionCarta % 13 == 0)
            {
                valorCarta = 11;
            }
            if (posicionCarta % 13 ==1)
            {
                valorCarta = 1;
            }
            if (!(valorCarta >= 10))
            {
                valorCarta++;
            }

          

           
        
            posicionCarta++;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        int cont = 0;
        for (int i = 0; i <51 ; i++)
        {
            valuesRandom[i] = 0;
        }

        while (cont != values.Length-1)
        {
            Debug.Log(cont);
            int valorRandom = Random.Range(0, values.Length);

            if (valuesRandom[valorRandom] == 0)
            {
                valuesRandom[valorRandom] = values[cont];
                facesRandom[valorRandom] = faces[cont];
                cont++;
            }
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            if (player.GetComponent<CardHand>().points == 21 )
            {
                finalMessage.text = "Has ganado";
                End();
            }

            if(dealer.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Has perdido";
                End();
            }
        }
    }

    private void CalculateProbabilities()
    {
        float cartasActuales = 52- cardIndex;
        float diferencia = player.GetComponent<CardHand>().points - dealer.GetComponent<CardHand>().points;

        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        float DealerMasJugador()
        {
            float casosFavorables = 0;          

            if (diferencia < 0){

                return 1;
            }
            else if (player.GetComponent<CardHand>().points == dealer.GetComponent<CardHand>().points)
            {
                Debug.Log(player.GetComponent<CardHand>().points +" "+ dealer.GetComponent<CardHand>().points);
                return 0;
            }
            else
            {
                for (int i = cardIndex-1; i < valuesRandom.Length; i++)
                {
                    if (valuesRandom[i] >= diferencia)
                    {
                        casosFavorables++;
                    }
                }               

                return casosFavorables/cartasActuales;
            }
        }
         
        
        float entre17y21()
        {
            float casosFavorables = 0;
       

            for (int i = cardIndex - 1; i < valuesRandom.Length; i++)
            {
                float ActPtosJugador= player.GetComponent<CardHand>().points + valuesRandom[i];
               
                if ((ActPtosJugador >= 17) && (ActPtosJugador <= 21))
                {
                    casosFavorables++;
                }

            }
            return casosFavorables / cartasActuales;

        }

        float Mas21()
        {
            float casosFavorables = 0;
            for (int i = cardIndex - 1; i < valuesRandom.Length; i++)
            {
                float NuevosPtosJugador= player.GetComponent<CardHand>().points + valuesRandom[i];

                if(NuevosPtosJugador > 21)
                {
                    casosFavorables++;
                }
            }
            return casosFavorables / cartasActuales;
        }
        probMessage.text = DealerMasJugador().ToString();
        probMessage2.text = entre17y21().ToString();
        probMessage3.text = Mas21().ToString();
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        //hecho
        dealer.GetComponent<CardHand>().Push(facesRandom[cardIndex], valuesRandom[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        //hecho
        player.GetComponent<CardHand>().Push(facesRandom[cardIndex], valuesRandom[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();

        int PtosJugador = player.GetComponent<CardHand>().points;


        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (PtosJugador > 21)
        {
            finalMessage.text = "Has perdido";
            End();

        }
        if (PtosJugador == 21)
        {
            finalMessage.text = "Has ganado";

            End();
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        while (dealer.GetComponent<CardHand>().points <= 16 )
        {
            PushDealer();
        }
        if (dealer.GetComponent<CardHand>().points >= 17 || dealer.GetComponent<CardHand>().points <= 21)
        {
           
            if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
            {
                finalMessage.text = "Empate";
                End();
                
            }
            if (player.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points)
            {
                finalMessage.text = "Has ganado";
                End();
               
            }
            if (player.GetComponent<CardHand>().points < dealer.GetComponent<CardHand>().points)
            {
                finalMessage.text = "Has perdido";
                End();
               
            }
        }
        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has ganado";
        }
    }

    public void End()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        hitButton.interactable = false;
        stickButton.interactable = false;
    }
    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        probMessage.text = "";
        probMessage2.text = "";
        probMessage3.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}
