'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function ManageStaff() {
  const [staff, setStaff] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddForm, setShowAddForm] = useState(false);
  const [formData, setFormData] = useState({
    staffID: '',
    fullName: '',
    email: '',
    department: '',
    role: 'Advisor',
  });

  useEffect(() => {
    fetchStaff();
  }, []);

  const fetchStaff = async () => {
    try {
      const response = await api.get('/admin/staff');
      setStaff(response.data);
    } catch (error) {
      toast.error('Failed to load staff');
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/admin/staff', formData);
      toast.success('Staff added successfully');
      setShowAddForm(false);
      setFormData({
        staffID: '',
        fullName: '',
        email: '',
        department: '',
        role: 'Advisor',
      });
      fetchStaff();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Failed to add staff');
    }
  };

  const handleDelete = async (staffId: string) => {
    if (!confirm(`Are you sure you want to delete staff ${staffId}?`)) return;
    try {
      await api.delete(`/admin/staff/${staffId}`);
      toast.success('Staff deleted successfully');
      fetchStaff();
    } catch (error) {
      toast.error('Failed to delete staff');
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
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Manage Staff (Base Database)</h1>
          <button
            onClick={() => setShowAddForm(!showAddForm)}
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          >
            {showAddForm ? 'Cancel' : 'Add Staff'}
          </button>
        </div>

        {showAddForm && (
          <form onSubmit={handleAdd} className="mb-6 p-4 border rounded-lg bg-gray-50">
            <h3 className="font-semibold mb-4">Add New Staff</h3>
            <div className="grid md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Staff ID *</label>
                <input
                  type="text"
                  value={formData.staffID}
                  onChange={(e) => setFormData({ ...formData, staffID: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  placeholder="ADV001"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Full Name *</label>
                <input
                  type="text"
                  value={formData.fullName}
                  onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Email *</label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  placeholder="name@aastu.edu.et"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Department *</label>
                <input
                  type="text"
                  value={formData.department}
                  onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  placeholder="Computer Science"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Role *</label>
                <select
                  value={formData.role}
                  onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                >
                  <option value="Advisor">Advisor</option>
                  <option value="DepartmentHead">Department Head</option>
                  <option value="Registrar">Registrar</option>
                  <option value="CostSharingOfficer">Cost Sharing Officer</option>
                  <option value="SystemAdmin">System Admin</option>
                </select>
              </div>
            </div>
            <button
              type="submit"
              className="mt-4 bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
            >
              Add Staff
            </button>
          </form>
        )}

        <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded">
          <p className="text-sm text-yellow-800">
            <strong>Note:</strong> Only staff in this base database can sign up for accounts. 
            This is how the system validates university membership.
          </p>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2">Staff ID</th>
                <th className="border p-2">Full Name</th>
                <th className="border p-2">Email</th>
                <th className="border p-2">Department</th>
                <th className="border p-2">Role</th>
                <th className="border p-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {staff.map((s) => (
                <tr key={s.staffID}>
                  <td className="border p-2">{s.staffID}</td>
                  <td className="border p-2">{s.fullName}</td>
                  <td className="border p-2">{s.email}</td>
                  <td className="border p-2">{s.department}</td>
                  <td className="border p-2">{s.role}</td>
                  <td className="border p-2">
                    <button
                      onClick={() => handleDelete(s.staffID)}
                      className="text-red-600 hover:underline text-sm"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {staff.length === 0 && (
          <p className="text-gray-600 mt-4">No staff found. Add staff to allow them to sign up.</p>
        )}
      </div>
    </Layout>
  );
}
