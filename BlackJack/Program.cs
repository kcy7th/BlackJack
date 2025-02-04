using System;
using System.Collections.Generic;

public enum Suit { Hearts, Diamonds, Clubs, Spades }
public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

// 카드 한 장을 표현하는 클래스
public class Card
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }

    public Card(Suit s, Rank r)
    {
        Suit = s;
        Rank = r;
    }

    public int GetValue()
    {
        if ((int)Rank <= 10)
        {
            return (int)Rank;
        }
        else if ((int)Rank <= 13)
        {
            return 10;
        }
        else
        {
            return 11;
        }
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

// 덱을 표현하는 클래스
public class Deck
{
    private List<Card> cards;

    public Deck()
    {
        cards = new List<Card>();

        foreach (Suit s in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new Card(s, r));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        Random rand = new Random();

        for (int i = 0; i < cards.Count; i++)
        {
            int j = rand.Next(i, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    public Card DrawCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}

// 패를 표현하는 클래스
public class Hand
{
    private List<Card> cards;

    public Hand()
    {
        cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public int GetTotalValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in cards)
        {
            if (card.Rank == Rank.Ace)
            {
                aceCount++;
            }
            total += card.GetValue();
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    public override string ToString()
    {
        return string.Join(", ", cards);
    }
}

// 플레이어를 표현하는 클래스
public class Player
{
    public Hand Hand { get; set; }

    public Player()
    {
        Hand = new Hand();
    }

    public Card DrawCardFromDeck(Deck deck)
    {
        Card drawnCard = deck.DrawCard();
        Hand.AddCard(drawnCard);
        return drawnCard;
    }

    public int GetTotalValue()
    {
        return Hand.GetTotalValue();
    }
}


// 딜러 턴 로직
public class Dealer : Player
{
    public void PlayTurn(Deck deck)
    {
        while (GetTotalValue() < 17)
        {
            Console.WriteLine("Dealer draws a card.");
            DrawCardFromDeck(deck);  // 카드 한 장 더 받기
        }
    }
}

// 블랙잭 게임 구현
public class Blackjack
{
    private Deck deck;
    private Player player;
    private Dealer dealer;

    public Blackjack()
    {
        InitializeGame();
    }

    // 게임을 초기화하는 메서드
    private void InitializeGame()
    {
        deck = new Deck();  // 새로운 덱 생성
        player = new Player();  // 플레이어 손 초기화
        dealer = new Dealer();  // 딜러 손 초기화
    }

    public void StartGame()
    {
        // 게임 시작 시 매번 손을 초기화하고, 새로운 카드 받기
        player.Hand = new Hand();  // 플레이어 손 초기화
        dealer.Hand = new Hand();  // 딜러 손 초기화

        // 초기 카드 두 장씩 나누기
        player.DrawCardFromDeck(deck);
        player.DrawCardFromDeck(deck);
        dealer.DrawCardFromDeck(deck);
        dealer.DrawCardFromDeck(deck);

        // 게임 진행
        Console.WriteLine("Player's turn:");
        PlayerTurn();

        if (player.Hand.GetTotalValue() <= 21) // 플레이어가 21을 넘지 않았다면
        {
            Console.WriteLine("Dealer's turn:");
            dealer.PlayTurn(deck);
        }

        // 결과 출력
        ShowResult();
    }

    private void PlayerTurn()
    {
        while (true)
        {
            Console.WriteLine($"Your hand: {string.Join(", ", player.Hand)} (Total: {player.Hand.GetTotalValue()})");
            Console.WriteLine("Do you want to hit or stay? (h/s)");

            string choice = Console.ReadLine();
            if (choice == "h") // 카드를 더 받는 경우
            {
                player.DrawCardFromDeck(deck);
                if (player.Hand.GetTotalValue() > 21)
                {
                    Console.WriteLine("Your hand is over 21! You bust!");
                    break;
                }
            }
            else if (choice == "s") // 카드를 그만 받는 경우
            {
                break;
            }
        }
    }

    private void ShowResult()
    {
        int playerTotal = player.Hand.GetTotalValue();
        int dealerTotal = dealer.Hand.GetTotalValue();

        Console.WriteLine($"Player's final hand: {string.Join(", ", player.Hand)} (Total: {playerTotal})");
        Console.WriteLine($"Dealer's final hand: {string.Join(", ", dealer.Hand)} (Total: {dealerTotal})");

        if (playerTotal > 21)
        {
            Console.WriteLine("You busted! Dealer wins!");
        }
        else if (dealerTotal > 21)
        {
            Console.WriteLine("Dealer busted! You win!");
        }
        else if (playerTotal > dealerTotal)
        {
            Console.WriteLine("You win!");
        }
        else if (playerTotal < dealerTotal)
        {
            Console.WriteLine("Dealer wins!");
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Blackjack game = new Blackjack();
        while (true)
        {
            game.StartGame();
            Console.WriteLine("Do you want to play again? (y/n)");
            string replay = Console.ReadLine();
            if (replay.ToLower() != "y")
            {
                break;
            }
        }
    }
}