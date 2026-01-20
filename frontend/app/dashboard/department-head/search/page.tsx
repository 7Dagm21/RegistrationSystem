'use client';

import { useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';

export default function SearchStudent() {
  const [studentId, setStudentId] = useState('');
  const [student, setStudent] = useState<any>(null);
  const [loading, setLoading] = useState(false);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!studentId.trim()) {
      toast.error('Please enter a student ID');
      return;
    }

    setLoading(true);
    try {
      const response = await api.get(`/departmenthead/students/search?studentId=${studentId}`);
      setStudent(response.data);
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Student not found');
      setStudent(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout role="DepartmentHead">
      <div className="bg-white rounded-lg shadow p-6">
        <h1 className="text-2xl font-bold mb-6">Search Student</h1>
        
        <form onSubmit={handleSearch} className="mb-6">
          <div className="flex gap-4">
            <input
              type="text"
              value={studentId}
              onChange={(e) => setStudentId(e.target.value)}
              className="flex-1 px-3 py-2 border rounded-lg"
              placeholder="Enter Student ID"
            />
            <button
              type="submit"
              disabled={loading}
              className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Searching...' : 'Search'}
            </button>
          </div>
        </form>

        {student && (
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold text-lg mb-4">Student Profile</h3>
            <div className="grid md:grid-cols-2 gap-4">
              <p><strong>Student ID:</strong> {student.studentId}</p>
              <p><strong>Full Name:</strong> {student.fullName}</p>
              <p><strong>Email:</strong> {student.email}</p>
              <p><strong>Department:</strong> {student.department}</p>
              <p><strong>Enrollment Year:</strong> {student.enrollmentYear}</p>
              <p><strong>Academic Year:</strong> Year {student.academicYear}</p>
            </div>
          </div>
        )}
      </div>
    </Layout>
  );
}
