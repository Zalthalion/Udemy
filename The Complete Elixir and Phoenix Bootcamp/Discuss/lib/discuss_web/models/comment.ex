defmodule DiscussWeb.Comment do
  use DiscussWeb, :model

  schema "comments" do
    field :content, :string
    belongs_to :user, DiscussWeb.User
    belongs_to :topic, DiscussWeb.Topic

    timestamps()
  end

  def changeset(struct, params \\ %{}) do
    struct
    |> cast(params, [:content, :user, :topic])
    |> validate_required([:content, :user, :topic])
  end
end
