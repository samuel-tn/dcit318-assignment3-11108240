using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new List<T>(items);

        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription ID: {Id}, Medication: {MedicationName}, Date Issued: {DateIssued:d}";
        }
    }

    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();
        public void SeedData()
        {

            _patientRepo.Add(new Patient(1101, "Harriet Zoe", 23, "Female"));
            _patientRepo.Add(new Patient(1102, "Janet Adjei", 38, "Female"));
            _patientRepo.Add(new Patient(1103, "Samuel Zirkzee", 43, "Male"));

            _prescriptionRepo.Add(new Prescription(1, 1101, "LUFART", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(2, 1101, "PARACETAMOL", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 1102, "AMUZU GARLIC", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(4, 1103, "CITRO C", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(5, 1101, "COLODIUM", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("===== Patients =====");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId)
                ? _prescriptionMap[patientId]
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"\n=== Prescriptions for Patient ID: {patientId} ===");

            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
            }
            else
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            app.SeedData();

            app.BuildPrescriptionMap();

            app.PrintAllPatients();

            int selectedPatientId = 1102;
            app.PrintPrescriptionsForPatient(selectedPatientId);

        }
    }
}

