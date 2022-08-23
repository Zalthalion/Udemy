defmodule CardsTest do
  use ExUnit.Case
  doctest Cards

  test "create_deck creates 52 cards" do
    card_count = length(Cards.create_deck)
    assert card_count == 52
  end

  test "shuffle actually shuffles" do
    deck = Cards.create_deck
    refute deck == Cards.shuffle(deck)
  end
end
