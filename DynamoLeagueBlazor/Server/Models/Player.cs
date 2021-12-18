﻿namespace DynamoLeagueBlazor.Server.Models;

public record Player : BaseEntity
{
    public Player(string name, string position, string headShot)
    {
        Name = name;
        Position = position;
        HeadShot = headShot;
    }

    public string Name { get; private set; }
    public string Position { get; private set; }
    public string HeadShot { get; private set; }
    public int? ContractLength { get; set; } // TODO: Change to a DateTime
    public int ContractValue { get; set; }
    public int YearAcquired { get; set; }
    public bool Rostered { get; set; }
    public int? TeamId { get; set; }
    public DateTime? EndOfFreeAgency { get; set; }

    public Team Team { get; private set; } = null!;
    public ICollection<Bid> Bids { get; private set; } = new HashSet<Bid>();
    public ICollection<Fine> Fines { get; private set; } = new HashSet<Fine>();

    // TODO: State Machine these
    public Player SetToRostered(DateTime contractedToDate, int contractValue)
    {
        Rostered = true;
        ContractLength = contractedToDate.Year;
        EndOfFreeAgency = null;
        ContractValue = contractValue;

        return this;
    }

    public Player SetToUnrostered()
    {
        Rostered = false;
        EndOfFreeAgency = null;

        return this;
    }

    public Player SetToUnsigned()
    {
        Rostered = false;
        ContractLength = null;
        EndOfFreeAgency = null;
        YearAcquired = DateTime.Today.Year;

        return this;
    }

    public Player SetToFreeAgent(DateTime endOfFreeAgency)
    {
        EndOfFreeAgency = endOfFreeAgency;

        return this;
    }
}

public static class PlayerExtensions
{
    public static IQueryable<Player> WhereIsRostered(this IQueryable<Player> players)
        => players.Where(p => p.Rostered
            && p.ContractLength > DateTime.Today.Year
            && p.EndOfFreeAgency == null);
    public static IQueryable<Player> WhereIsUnrostered(this IQueryable<Player> players)
        => players.Where(p => p.Rostered == false
            && p.ContractLength != null
            && p.EndOfFreeAgency == null);

    public static IQueryable<Player> WhereIsUnsigned(this IQueryable<Player> players)
        => players.Where(p => p.Rostered == false
            && p.ContractLength == null
            && p.EndOfFreeAgency == null
            && p.YearAcquired == DateTime.Today.Year);

    public static IQueryable<Player> WhereIsEligibleForFreeAgency(this IQueryable<Player> players)
        => players.Where(p => p.ContractLength < DateTime.Today.Year);

    public static IQueryable<Player> WhereIsFreeAgent(this IQueryable<Player> players)
        => players.Where(p => p.ContractLength < DateTime.Today.Year
            && p.EndOfFreeAgency >= DateTime.Now);
}