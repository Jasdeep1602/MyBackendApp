// Services/ListItemService.cs
using MongoDB.Driver;
using MyBackendApp.Configurations;

public class ListItemService
{
    private readonly IMongoCollection<ListItem> _listItems;

public ListItemService(IConfiguration config)
{
    // Retrieve the DatabaseSettings section and bind it to the DatabaseSettings object
    var databaseSettings = new DatabaseSettings();
    config.GetSection("DatabaseSettings").Bind(databaseSettings);

    // Initialize MongoDB client, database, and collection
    var client = new MongoClient(databaseSettings.ConnectionString);
    var database = client.GetDatabase(databaseSettings.DatabaseName);
    _listItems = database.GetCollection<ListItem>("ListItems");
}


    public async Task<List<ListItem>> GetAllAsync() => 
        await _listItems.Find(_ => true).ToListAsync();

    public async Task<ListItem?> GetAsync(string id) =>
        await _listItems.Find(item => item.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(ListItem newItem) =>
        await _listItems.InsertOneAsync(newItem);

    public async Task UpdateAsync(string id, ListItem updatedItem) =>
        await _listItems.ReplaceOneAsync(item => item.Id == id, updatedItem);

    public async Task DeleteAsync(string id) =>
        await _listItems.DeleteOneAsync(item => item.Id == id);
}
