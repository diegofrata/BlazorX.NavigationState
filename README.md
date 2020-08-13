![Nuget](https://img.shields.io/nuget/v/BlazorX.NavigationState)
# BlazorX.NavigationState
BlazorX.NavigationState provides a set of utilities that allow you to observe and bind query strings to Blazor components.

### Getting Started

1. Add the package BlazorX.NavigationState to your Blazor project.
2. Register BlazorX.NavigationState with your host builder as follows:

    ```csharp
    builder.Services.AddNavigationState();
    ```
3. Add the namespace BlazorX.NavigationState to your _Imports.razor.

### Query Property

You can use the method QueryProperty on NavigationState to create an object 
that will track a given query string parameter. With this object you can bind
to controls just like a normal value. 

In the below example, anything the user types in the input will be reflected in
the URL (eg. http://localhost/?name=Diego) and vice-versa.

 ```razor
@inject INavigationState NavigationState

<input type="text" @bind-Value="Name.Value" />

@code {
    IQueryParameter<string> Name;
    
    protected override void OnInitialized() 
    {
        Name = NavigationState.QueryProperty("name", "");
    }
}
```


### Query Array

The QueryArray class deals with query strings that can appear multiple
times in the URL. It's useful to deal with collection of things. 

 ```razor
@inject INavigationState NavigationState

<ul>
@foreach (var item in Numbers.Value) 
{
    <li>@item</li>
}
</ul>


@code {
    IQueryParameter<int> Numbers;
    
    protected override void OnInitialized() 
    {
        Numbers = NavigationState.QueryArray("numbers", new int[0]);
    }
}
```

### Observable

Both QueryProperty and QueryArray supports observables via the property ValueStream:

 ```razor
@inject INavigationState NavigationState

@Sum
<button @onclick="@(() => Number.Value += 1)">Increment</button> 

@code {
    IQueryParameter<int> Number;
    int Sum;
    
    protected override void OnInitialized() 
    {
        Number = NavigationState.QueryProperty("n", 0);
        
        Number.ValueStream.Subscribe(x => 
        {
            Sum += x;
            StateChanged();
        });
    }
}
 ```
