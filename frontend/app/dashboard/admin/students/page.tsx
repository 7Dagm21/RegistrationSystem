'use client';

import { useEffect, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function ManageStudents() {
  const [students, setStudents] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddForm, setShowAddForm] = useState(false);
  const [formData, setFormData] = useState({
    studentID: '',
    fullName: '',
    universityEmail: '',
    enrollmentYear: new Date().getFullYear(),
    department: '',
    role: 'Student',
  });

  useEffect(() => {
    fetchStudents();
  }, []);

  const fetchStudents = async () => {
    try {
      const response = await api.get('/admin/students');
      setStudents(response.data);
    } catch (error) {
      toast.error('Failed to load students');
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await api.post('/admin/students', formData);
      toast.success('Student added successfully');
      setShowAddForm(false);
      setFormData({
        studentID: '',
        fullName: '',
        universityEmail: '',
        enrollmentYear: new Date().getFullYear(),
        department: '',
        role: 'Student',
      });
      fetchStudents();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Failed to add student');
    }
  };

  const handleDelete = async (studentId: string) => {
    if (!confirm(`Are you sure you want to delete student ${studentId}?`)) return;
    try {
      await api.delete(`/admin/students/${studentId}`);
      toast.success('Student deleted successfully');
      fetchStudents();
    } catch (error) {
      toast.error('Failed to delete student');
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
          <h1 className="text-2xl font-bold">Manage Students (Base Database)</h1>
          <button
            onClick={() => setShowAddForm(!showAddForm)}
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          >
            {showAddForm ? 'Cancel' : 'Add Student'}
          </button>
        </div>

        {showAddForm && (
          <form onSubmit={handleAdd} className="mb-6 p-4 border rounded-lg bg-gray-50">
            <h3 className="font-semibold mb-4">Add New Student</h3>
            <div className="grid md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Student ID *</label>
                <input
                  type="text"
                  value={formData.studentID}
                  onChange={(e) => setFormData({ ...formData, studentID: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  placeholder="STU001"
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
                <label className="block text-sm font-medium mb-2">University Email *</label>
                <input
                  type="email"
                  value={formData.universityEmail}
                  onChange={(e) => setFormData({ ...formData, universityEmail: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  placeholder="name@aastustudent.edu.et"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Enrollment Year *</label>
                <input
                  type="number"
                  value={formData.enrollmentYear}
                  onChange={(e) => setFormData({ ...formData, enrollmentYear: parseInt(e.target.value) })}
                  className="w-full px-3 py-2 border rounded-lg"
                  required
                  min="2000"
                  max="2030"
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
            </div>
            <button
              type="submit"
              className="mt-4 bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
            >
              Add Student
            </button>
          </form>
        )}

        <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded">
          <p className="text-sm text-yellow-800">
            <strong>Note:</strong> Only students in this base database can sign up for accounts. 
            This is how the system validates university membership.
          </p>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full border-collapse border">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2">Student ID</th>
                <th className="border p-2">Full Name</th>
                <th className="border p-2">Email</th>
                <th className="border p-2">Enrollment Year</th>
                <th className="border p-2">Department</th>
                <th className="border p-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.studentID}>
                  <td className="border p-2">{student.studentID}</td>
                  <td className="border p-2">{student.fullName}</td>
                  <td className="border p-2">{student.universityEmail}</td>
                  <td className="border p-2">{student.enrollmentYear}</td>
                  <td className="border p-2">{student.department}</td>
                  <td className="border p-2">
                    <button
                      onClick={() => handleDelete(student.studentID)}
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

        {students.length === 0 && (
          <p className="text-gray-600 mt-4">No students found. Add students to allow them to sign up.</p>
        )}
      </div>
    </Layout>
  );
}
