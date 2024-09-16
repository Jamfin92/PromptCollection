// React Service Component Example (Vite Project)
// File: src/components/UserList.tsx

import React, { useState, useEffect } from 'react';

interface User {
  id: number;
  name: string;
}

// In a real Vite project, this service would typically be in a separate file
// e.g., src/services/userService.ts
const UserService = {
  getUsers: async (): Promise<User[]> => {
    // Simulating API call
    return [
      { id: 1, name: 'Alice' },
      { id: 2, name: 'Bob' }
    ];
  }
};

const UserList: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    const fetchUsers = async () => {
      const fetchedUsers = await UserService.getUsers();
      setUsers(fetchedUsers);
    };
    fetchUsers();
  }, []);

  return (
    <div>
      <h2>User List</h2>
      <ul>
        {users.map(user => (
          <li key={user.id}>{user.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default UserList;

// Usage in App.tsx
import React from 'react';
import UserList from './components/UserList';

const App: React.FC = () => {
  return (
    <div>
      <h1>My Vite React App</h1>
      <UserList />
    </div>
  );
};

export default App;

// Blazor Service Component Example (unchanged)
@page "/"
@inject UserService UserService
@using System.Collections.Generic

<h2>User List</h2>
<ul>
    @foreach (var user in users)
    {
        <li>@user.Name</li>
    }
</ul>

@code {
    private List<User> users = new List<User>();

    protected override async Task OnInitializedAsync()
    {
        users = await UserService.GetUsersAsync();
    }
}

// UserService.cs
public class UserService
{
    public async Task<List<User>> GetUsersAsync()
    {
        // Simulating API call
        return new List<User>
        {
            new User { Id = 1, Name = "Alice" },
            new User { Id = 2, Name = "Bob" }
        };
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
