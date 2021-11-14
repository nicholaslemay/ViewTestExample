using System.Collections.Generic;
using ExampleWebApplication.Players;
using FluentAssertions;
using ViewTestExample.Support;
using Xunit;

namespace ViewTestExample.Views
{
    public class PlayerTest
    {
        public class LikeButtonWidgetTestCshtml : ViewTest
        {
            [Fact]
            public void AfficheNombreDeLikesActuels()
            {
                var viewData = new List<KeyValuePair<string, object>>
                {
                    new("extraData", "MyExtraData")
                };

                var rendered = RenderView("Players/Views/_Player.cshtml", new HockeyPlayer(66, "Mario Lemieux"), viewData);

                rendered.GetElementById("playerNumber").TextContent.Should().Contain("66");
                rendered.GetElementById("playerName").TextContent.Should().Contain("Mario Lemieux");
                rendered.GetElementById("playerName").GetAttribute("data-for-player-number").Should().Be("66");

                rendered.GetElementById("partial").TextContent.Should().Contain("partial content");

                rendered.GetElementById("link").GetAttribute("href").Should().Be("/players/66");
                rendered.GetElementById("extraData").TextContent.Should().Be("MyExtraData");
            }
        }
    }
}