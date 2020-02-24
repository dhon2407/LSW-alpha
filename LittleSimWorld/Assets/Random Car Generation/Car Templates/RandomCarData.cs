using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data holders for the random cars.
/// </summary>
/// This is for memory optimization.
/// When we generate 100 cars that each hold 8 floats and 4 references, it's more worth to have them hold just a single reference to a manager that holds all the related data.
/// TODO: Implement
public class RandomCarData : ScriptableObject
{


}
