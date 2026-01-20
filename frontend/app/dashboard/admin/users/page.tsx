'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function UserManagement() {
  const [users, setUsers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/admin/users');
      setUsers(response.data);
    } catch (error) {
      toast.error('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = async (userId: number) => {
    if (!confirm('Are you sure you want to reset this user?')) return;
    try {
      await api.put(`/admin/users/${userId}/reset`);
      toast.success('User reset successfully');
      fetchUsers();
    } catch (error) {
      toast.error('Failed to reset user');
    }
  };

  const handleDeactivate = async (userId: number) => {
    if (!confirm('Are you sure you want to deactivate this user?')) return;
    try {
      await api.put(`/admin/users/${userId}/deactivate`);
      toast.success('User deactivated');
      fetchUsers();
    } catch (error) {
      toast.error('Failed to deactivate user');
    }
  };

  if (loading) {
    return (
      <Layout role="SystemAdmin">
        <div>Loading...</div>
      </Layout>
    );
  }

  return (
    <Layout role="SystemAdmin">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">User Management</h1>
        
        <div className="overflow-x-auto">
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2">ID</th>
                <th className="border p-2">Name</th>
                <th className="border p-2">User ID</th>
                <th className="border p-2">Email</th>
                <th className="border p-2">Role</th>
                <th className="border p-2">Department</th>
                <th className="border p-2">Status</th>
                <th className="border p-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td className="border p-2">{user.id}</td>
                  <td className="border p-2">{user.fullName}</td>
                  <td className="border p-2">{user.userId}</td>
                  <td className="border p-2">{user.email}</td>
                  <td className="border p-2">{user.role}</td>
                  <td className="border p-2">{user.department || 'N/A'}</td>
                  <td className="border p-2">
                    <span className={user.isActive ? 'text-green-600' : 'text-red-600'}>
                      {user.isActive ? 'Active' : 'Inactive'}
                    </span>
                    {user.isEmailVerified && (
                      <span className="ml-2 text-blue-600">✓ Verified</span>
                    )}
                  </td>
                  <td className="border p-2">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleReset(user.id)}
                        className="text-blue-600 hover:underline text-sm"
                      >
                        Reset
                      </button>
                      <button
                        onClick={() => handleDeactivate(user.id)}
                        className="text-red-600 hover:underline text-sm"
                      >
                        {user.isActive ? 'Deactivate' : 'Activate'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </Layout>
  );
}
