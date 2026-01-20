'use client';

import { useEffect, useMemo, useState } from 'react';
import Layout from '@/components/Layout';
import api from '@/lib/api';
import { toast } from 'react-toastify';
import Link from 'next/link';

type Course = { courseCode: string; courseName: string; creditHours: number };

export default function AdvisorCreateSlip() {
  const [studentID, setStudentID] = useState('');
  const [department, setDepartment] = useState('Computer Science');
  const [academicYear, setAcademicYear] = useState(4);
  const [semester, setSemester] = useState('Fall');
  const [slipSemesterLabel, setSlipSemesterLabel] = useState('2025-2026 Fall');

  const [courses, setCourses] = useState<Course[]>([]);
  const [selected, setSelected] = useState<Record<string, boolean>>({});
  const [loadingCourses, setLoadingCourses] = useState(false);
  const [creating, setCreating] = useState(false);

  const selectedCourses = useMemo(
    () => courses.filter((c) => selected[c.courseCode]),
    [courses, selected]
  );
  const totalCH = useMemo(
    () => selectedCourses.reduce((sum, c) => sum + (c.creditHours || 0), 0),
    [selectedCourses]
  );

  const fetchCourses = async () => {
    setLoadingCourses(true);
    try {
      const res = await api.get(
        `/advisor/courses?academicYear=${academicYear}&department=${encodeURIComponent(
          department
        )}&semester=${encodeURIComponent(semester)}`
      );
      setCourses(res.data || []);
      setSelected({});
    } catch (e: any) {
      toast.error(e.response?.data?.message || 'Failed to load courses');
    } finally {
      setLoadingCourses(false);
    }
  };

  useEffect(() => {
    fetchCourses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [academicYear, department, semester]);

  const toggle = (code: string) => {
    setSelected((prev) => ({ ...prev, [code]: !prev[code] }));
  };

  const createSlip = async () => {
    if (!studentID.trim()) return toast.error('Enter Student ID');
    if (selectedCourses.length === 0) return toast.error('Select at least one course');
    setCreating(true);
    try {
      const res = await api.post('/advisor/slips/create', {
        studentID,
        semester: slipSemesterLabel,
        courses: selectedCourses,
      });
      toast.success('Slip created. You can review/approve it now.');
      const slipId = res.data?.id;
      if (slipId) {
        window.location.href = `/dashboard/advisor/slips/${slipId}`;
      }
    } catch (e: any) {
      toast.error(e.response?.data?.message || 'Failed to create slip');
    } finally {
      setCreating(false);
    }
  };

  return (
    <Layout role="Advisor">
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Create Slip (Advisor)</h1>
          <Link href="/dashboard/advisor/pending" className="text-blue-600 hover:underline">
            Go to Pending →
          </Link>
        </div>

        <div className="grid md:grid-cols-2 gap-4 mb-6">
          <div>
            <label className="block text-sm font-medium mb-2">Student ID</label>
            <input
              value={studentID}
              onChange={(e) => setStudentID(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg"
              placeholder="ETS0358/15"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Slip Semester Label</label>
            <input
              value={slipSemesterLabel}
              onChange={(e) => setSlipSemesterLabel(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg"
              placeholder="2025-2026 Fall"
            />
            <p className="text-xs text-gray-500 mt-1">
              This is the value stored on the slip (e.g. “2025-2026 Fall”).
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Department</label>
            <input
              value={department}
              onChange={(e) => setDepartment(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg"
              placeholder="Computer Science"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Academic Year</label>
            <select
              value={academicYear}
              onChange={(e) => setAcademicYear(parseInt(e.target.value))}
              className="w-full px-3 py-2 border rounded-lg"
            >
              <option value={4}>Year 4</option>
              <option value={1}>Year 1</option>
              <option value={2}>Year 2</option>
              <option value={3}>Year 3</option>
              <option value={5}>Year 5</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Semester (Course Catalog)</label>
            <select
              value={semester}
              onChange={(e) => setSemester(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg"
            >
              <option value="Fall">First Semester (Fall)</option>
              <option value="Spring">Second Semester (Spring)</option>
              <option value="Summer">Summer</option>
            </select>
          </div>
        </div>

        <div className="mb-4 flex items-center gap-3">
          <button
            onClick={fetchCourses}
            className="bg-gray-800 text-white px-4 py-2 rounded hover:bg-gray-900"
            disabled={loadingCourses}
          >
            {loadingCourses ? 'Loading...' : 'Reload Courses'}
          </button>
          <div className="text-sm text-gray-600">
            Selected: <b>{selectedCourses.length}</b> course(s), Total: <b>{totalCH}</b> CH
          </div>
        </div>

        <div className="border rounded-lg">
          <div className="p-3 bg-gray-50 border-b font-semibold">Courses</div>
          {loadingCourses ? (
            <div className="p-4">Loading courses...</div>
          ) : courses.length === 0 ? (
            <div className="p-4 text-gray-600">
              No courses found for Year {academicYear}, {department}, {semester}.
              <div className="mt-2 text-sm">
                Add them into the `Courses` table first (Admin/DepartmentHead), then reload.
              </div>
            </div>
          ) : (
            <div className="divide-y">
              {courses.map((c) => (
                <label key={c.courseCode} className="flex items-center gap-3 p-3 hover:bg-gray-50">
                  <input
                    type="checkbox"
                    checked={!!selected[c.courseCode]}
                    onChange={() => toggle(c.courseCode)}
                  />
                  <div className="flex-1">
                    <div className="font-semibold">
                      {c.courseCode} — {c.courseName}
                    </div>
                    <div className="text-sm text-gray-600">{c.creditHours} credit hours</div>
                  </div>
                </label>
              ))}
            </div>
          )}
        </div>

        <div className="mt-6">
          <button
            onClick={createSlip}
            disabled={creating}
            className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {creating ? 'Creating...' : 'Create Slip'}
          </button>
          <p className="text-sm text-gray-500 mt-2">
            After creation, you will be redirected to the slip review page so you can approve/reject.
          </p>
        </div>
      </div>
    </Layout>
  );
}

