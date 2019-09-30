## SimpleAggregate has moved
For a more robust and elegant solution see https://github.com/jpovey/SimpleAggregate



# SimpleAggregate V1
A package to help simplify applying events to a DDD aggregate.

# How to use
## Aggregates
1. Create an aggregate which inherits from `AggregateBase`
2. Setup event handlers
3. Register event handlers

```c#
// Step 1
public class SavingsAccount : AggregateBase 
{
    public string AccountId { get; private set; }
    public int SavingsTarget { get; private set; }

    public SavingsAccount()
    {
        // Step 3
        this.RegisterEventHandler<SavingsAccountCreated>(Handle);
    }     

    // Step 2
    private void Handle(SavingsAccountCreated savingsAccountCreated)
    {
        AccountId = savingsAccountCreated.AccountId;
        SavingsTarget = savingsAccountCreated.SavingsTarget;
    }
}
```

4. Declare a new instance of the aggregate
```c#
// Step 4
var account = new SavingsAccount();
```
5. Supply events to hyrdate the aggregate
```c#
// Step 5
var events = new List<object>
{
    new SavingsAccountCreated(_accountId, SavingsTarget)
};

account.Hydrate(events);
```

## Aggregate Repository
The aggregate repository can be used to load an aggregate from an injected event source such as cosmosDb. The AggregateRepository requires an instance of IEventSource which is used to wrap the event store data access implementation.

This class could be used in a command handler to perform some business logic against an aggregate.

```c#
public class AccountHandler 
{
    private readonly AggregateRepository _repository;

    public AccountHandler(AggregateRepository repository) 
    {
        _repository = repository
    }

    public void Handle(AddSavingsCommand command) 
    {
        var aggregate = await _repository.GetAggregate<SavingsAccount>(command.accountId);
        aggregate.AddSavings(command.Amount);
        _repository.Save(aggregate)
    }
}

```
