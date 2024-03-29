namespace RuDiscordUtils;

using Discord;
using Discord.WebSocket;

public static class EmbedFactory
{
    public static Color DefaultEmbedColor { get; set; } = new Color(0x00ce6c);
    public static bool HasDefaultTimestamp {get; set; } = true;
    public static bool HasDefaultFooter {get; set; } = true;

    public static EmbedBuilder CreateBuilder(DiscordSocketClient? client)
    {
        EmbedBuilder embed = new EmbedBuilder();

        embed.WithColor(DefaultEmbedColor);

        if (HasDefaultTimestamp)
        {
             embed.WithCurrentTimestamp();
        }

        if (client != null && HasDefaultFooter)
        {
            embed.WithFooter(client.CurrentUser.Username, client.CurrentUser.GetAvatarUrl());
        }
       
        return embed;
    }
}