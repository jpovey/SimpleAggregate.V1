# SimpleAggregate
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
The aggregate repository can be used to load an aggregate from an injected event source such as cosmosDb. The IEventSource should be used to wrap the internal data access implementation.

```c#
var eventSource = new EventSource(); //IEventSource wrapper
var repository = new AggregateRepository(eventSource);

var accountId = "1234";
var aggregate = await repository.GetAggregate<SavingsAccount>(accountId);
aggregate.AddSavings(50);
repository.Save(aggregate)
```