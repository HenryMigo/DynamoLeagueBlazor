﻿using Humanizer;
using Humanizer.Localisation;
using System.Timers;
using Timer = System.Timers.Timer;

namespace DynamoLeagueBlazor.Client.Features.FreeAgents.Detail;

public partial class BidCountdown
{
    [Parameter, EditorRequired] public DateTime DateTime { get; set; }

    private readonly Timer _timer = new(1000);
    private string _remainingTime = string.Empty;
    private Color _color = Color.Warning;

    protected override void OnInitialized()
    {
        _timer.Elapsed += CountDown;
        _timer.Enabled = true;
    }

    private void CountDown(object? source, ElapsedEventArgs e)
    {
        var remainingTime = DateTime - DateTime.Now;

        if (remainingTime <= TimeSpan.FromDays(1))
        {
            _color = Color.Error;

            if (remainingTime <= TimeSpan.Zero)
            {
                _timer.Enabled = false;
                remainingTime = TimeSpan.Zero;
            }
        }

        _remainingTime = remainingTime.Humanize(4, maxUnit: TimeUnit.Day, minUnit: TimeUnit.Second);

        InvokeAsync(StateHasChanged);
    }
}
