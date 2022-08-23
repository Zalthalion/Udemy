defmodule Cards do
  @moduledoc """
    Provides methods for creating and handling a deck of cards.
  """

  @doc """
  Returns a list of strings representing a deck of playing cards
  """
  def create_deck do
    values = ["Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"]
    suits = ["Spades", "Hearts", "Clubs", "Diamonds"]

    for suit <- suits, value <- values do
      "#{value} of #{suit}"
    end
  end

  @doc """
  Returns a list of a shuffled previously created deck
  """
  def shuffle(deck) do
    Enum.shuffle(deck)
  end

  @doc """
  Checks if the deck contains a certain card.

  ## Examples

      iex> deck = Cards.create_deck
      iex> Cards.contains?(deck, "Ace of Spades")
      true
  """
  def contains?(deck, card) do
    Enum.member?(deck, card)
  end

  @doc """
  Devides a deck into a hand and the remainder of the deck.
  The `hand_size`  argument indicates how many cards should be in a hand.

  ## Examples

      iex> deck = Cards.create_deck
      iex> {hand, deck} = Cards.deal(deck, 1)
      iex> hand
      ["Ace of Spades"]
  """
  def deal(deck, hand_size)do
    Enum.split(deck, hand_size)
  end

  @doc """
  Saves a certain deck to a provided `filename`
  """
  def save(deck, filename)do
    binary = :erlang.term_to_binary(deck)
    File.write(filename, binary)
  end

  @doc """
  Loads a deck from a provided `filename`
  """
  def load(filename)do
    case File.read(filename) do
      {:ok, binary} -> :erlang.binary_to_term(binary)
      {:error, _reason} -> "File does not exist"
    end
  end

  @doc """
  Crates a deck, shuffles the cards and deals them, based on `hand_size`
  """
  def create_hand(hand_size)do
    Cards.create_deck
    |> Cards.shuffle
    |> Cards.deal(hand_size)
  end
end
