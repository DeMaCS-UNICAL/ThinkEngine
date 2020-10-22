using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

/*
  * The TestingManager class
  * 1. acts as an intermediary between the framework window and the running game
  * 2. takes care of starting the tests entered by checking the status of the game object and the parameters set
  */

public class TestingManager : MonoBehaviour
{
    GameObject player; //tests are performed on this object
    object instanceTriggerConditions; // Contains the instance for calling Boolean functions of type triggerCondition
    List<object> instanceTests = new List<object>(); // It contains the instances to recall the various test

    // Init configuration from TestingWindow.cs
    void Start()
    {
        // At start-up, the TestingManager first 
        // takes care of loading the TestingWindow configuration through a loadData ()
        TestingWindow.loadData();
        player = TestingWindow.p_objectToTest;

        // If a triggerCondition script has been inserted it 
        //creates an instance of that class to invoke its functions
        if (TestingWindow.p_triggerConditionsScript != null)
        {
            Type type = Type.GetType(TestingWindow.p_triggerConditionsScript.name);
            instanceTriggerConditions = Activator.CreateInstance(type);
        }

        // Similarly, the instantiate operation is performed 
        // if there are any testing scripts and if any tests have been found
        if (TestingWindow.p_testScripts.Count > 0 && TestingWindow.p_tests.Count > 0)
        {
            foreach (Test test in TestingWindow.p_tests)
            {
                Type classeTest = Type.GetType(test.getMethodInfo().DeclaringType.ToString());
                object o = Activator.CreateInstance(classeTest);
                instanceTests.Add(o);

                // If there are scripts and tests loaded I start the coroutine for each test only if 
                // that test has been selected, configured correctly and put in the stop state for exceptional cases
                if (test.getSelected() && !test.getStopRunningTest() && !test.getBadSetting())
                    StartCoroutine(triggerCoroutine(test));
            }
        }
    }

    // This function calls the control function for the default trigger 
    // conditions and those (if any) created by the user for each test
    public IEnumerator triggerCoroutine(Test test)
    {
        // If the trigger conditions have not been met, you cannot start the test
        if (!checkDefaultTriggerConditions(test))
            yield return new WaitUntil(() => (bool)checkDefaultTriggerConditions(test));

        if (!checkCustomTriggerConditions(test))
            yield return new WaitUntil(() => (bool)checkCustomTriggerConditions(test));
    }

    // It constantly checks the status of the triggerConditions on a selected, 
    // correctly configured and NOT completed test.
    void Update()
    {
        for (int i = 0; i < TestingWindow.p_tests.Count; i++)
            if (TestingWindow.p_tests[i].getSelected() && !TestingWindow.p_tests[i].getBadSetting() && !TestingWindow.p_tests[i].getStopRunningTest())
                if (checkDefaultTriggerConditions(TestingWindow.p_tests[i]) && checkCustomTriggerConditions(TestingWindow.p_tests[i]))
                    // The parameter -> i, passed in the CheckStartTest () function indicates the reference 
                    //to the instance of the i-th position of the istanceTests list that can start the test.
                    checkStartTest(TestingWindow.p_tests[i], i);
    }

    // Check the default trigger conditions
    // Returns true if all are satisfied
    public bool checkDefaultTriggerConditions(Test test)
    {
        // The check can only take place if a test is in WAIT status. 
        // The WAIT state persists as long as a single trigger condition results in false
        if (test.getTestState() == TestState.WAIT)
        {
            // If a trigger condition of type position is active, 
            // it checks the X, Y and possibly Z coordinates
            if (test.getTriggerPosition())
            {
                // The check takes place if the coordinate has been monitored. 
                // A coordinate is monitored when a comparison operator is chosen.
                if (!test.getOperator(0).Equals(Operator.DONT_CHECK))
                    if (!checkOperationForX(test))
                        return false;

                if (!test.getOperator(1).Equals(Operator.DONT_CHECK))
                    if (!checkOperationForY(test))
                        return false;

                // For 3d - game
                if (!test.getOperator(2).Equals(Operator.DONT_CHECK) && TestingWindow.p_threeDimensionActive)
                    if (!checkOperationForZ(test))
                        return false;

            }

            // If a trigger condition of the level type has been activated, 
            // it takes the variable from the script inserted by the user to make the comparison 
            // and monitor the state of the level
            if (test.getTriggerLevel())
            {
                Type type = Type.GetType(TestingWindow.p_levelScript.name);
                FieldInfo fieldType = type.GetField(TestingWindow.p_levelScriptAttribute, BindingFlags.Public | BindingFlags.Instance);

                if (test.getLevel() != (int)fieldType.GetValue(player.GetComponent(TestingWindow.p_levelScript.name)))
                    return false;
            }
        }

        return true;
    }

    // Check the trigger conditions created by the user
    // Returns true if all are satisfied
    public bool checkCustomTriggerConditions(Test test)
    {
        // The check occurs if an instance has been set in the Start () and if the test is in the WAIT state
        if (instanceTriggerConditions != null && test.getTestState() == TestState.WAIT)
        {
            // Each test has its own list of activated trigger conditions
            foreach (TriggerCondition tc in test.getTriggerConditions())
            {
                // If the triggerCondition has been selected I have to check its status
                if (tc.getSelected())
                {
                    if (!(bool)tc.getMethodInfo().Invoke(instanceTriggerConditions, null))
                        return false;
                }
            }
        }

        return true;
    }


    // Given a test, it carries out an appropriate check to be carried out on the X,Y,Z axis 
    // based on the operator entered in the configuration phase
    public bool checkOperationForX(Test test) // X = 0, Y = 1, Z = 2, NOT_MONITORED = 4
    {
        if(test.getOperator(0) == Operator.GREATER_EQUAL)
            if (player.GetComponent<Transform>().position.x < double.Parse(test.getX()))
                return false;

        if (test.getOperator(0) == Operator.LESS_EQUAL)
            if (player.GetComponent<Transform>().position.x > double.Parse(test.getX()))
                return false;

        if (test.getOperator(0) == Operator.EQUAL)
            if (player.GetComponent<Transform>().position.x != double.Parse(test.getX()))
                return false;

        return true;
    }

    public bool checkOperationForY(Test test) 
    {
        if (test.getOperator(1) == Operator.GREATER_EQUAL)
            if (player.GetComponent<Transform>().position.y < double.Parse(test.getX()))
                return false;

        if (test.getOperator(1) == Operator.LESS_EQUAL)
            if (player.GetComponent<Transform>().position.y > double.Parse(test.getX()))
                return false;

        if (test.getOperator(1) == Operator.EQUAL)
            if (player.GetComponent<Transform>().position.y != double.Parse(test.getX()))
                return false;

        return true;
    }

    public bool checkOperationForZ(Test test)
    {
        if (test.getOperator(2) == Operator.GREATER_EQUAL)
            if (player.GetComponent<Transform>().position.z < double.Parse(test.getX()))
                return false;

        if (test.getOperator(2) == Operator.LESS_EQUAL)
            if (player.GetComponent<Transform>().position.z > double.Parse(test.getX()))
                return false;

        if (test.getOperator(2) == Operator.EQUAL)
            if (player.GetComponent<Transform>().position.z != double.Parse(test.getX()))
                return false;

        return true;
    }

    // This function is called when a test passes the trigger conditions. 
    // The parameter i indicates the instance of index i in the instanceTest list that can start the test
    public void checkStartTest(Test test, int i)
    {
        // for refresh setting
        // the refresh time is the time interval from when a test is executed and
        // gives a positive result  at its next execution
        if (Time.realtimeSinceStartup >= test.getNextRefreshTime() && (test.getTestState() == TestState.PASSED || test.getTestState() == TestState.FAILED))
        {
            test.updateNextRefreshTime(Time.realtimeSinceStartup + test.getRefreshTime());
            runTest(test, i);
        }
        // In this other case, if the test has never started (WAIT status) 
        // or a limitFrame has been set (RUN status), execute it without taking into account the refresh time. 
        // If the test results FAILED, its execution is blocked only if a limitFrame has not been set
        else if (test.getTestState() == TestState.RUN || test.getTestState() == TestState.WAIT)
            runTest(test, i);
    }

    // This function takes care of starting a test, calling it through the instanceTest [i] instance.
    // Before starting, any parameters must be read
    public void runTest(Test test, int i)
    {   
        bool result = true;
        object[] objects = convertParameters(test.getMethodInfo().GetParameters(), test);

        //  If the test is not an exception it means that everything is fine,
        // otherwise a message is set and shown through the interface
        try
        {
            test.getMethodInfo().Invoke(instanceTests[i], objects);
            test.setErrorMessage("none");
        }
        catch (Exception e)
        {
            result = false;
            test.setErrorMessage(e.GetBaseException().Message.ToString().Trim());
        }

        // Based on the result I change the status of the test
        // Possible changes
        // RUN->PASSED
        // RUN->FAILED
        // FAILED->PASSED
        // PASSED->FAILED
        if (result)
        {
            // If a fixed number of repetitions has been set, a value will be incremented and a stop () 
            // will be made by the incrementRipetitions () method when the repetitions reach the set limit.
            test.setTestState(TestState.PASSED);
            test.incrementRipetitions();
        }
        else
        {
            // If I set a limitFrame it is likely that the test will initially fail. 
            // In this case the state changes to running until the limitFrame is reached
            if (test.getTestState() == TestState.WAIT || test.getTestState() == TestState.RUN)
            {
                if (test.getFrameLimit() > 0 && test.incrementActualFrame())
                    test.setTestState(TestState.RUN);
                else //actual frame > frameLimit
                {
                    test.setTestState(TestState.FAILED);
                    if (TestingWindow.p_pauseTestFailedActive)
                        UnityEngine.Debug.Break();
                }
            }
            else test.setTestState(TestState.FAILED);
        }
    }

    // It deals with converting the parameters of a given test by taking the fields set in the configuration phase through reflection. 
    // Once fetched,  getValue () is performed through getComponent on the object to be tested.
    public object[] convertParameters(ParameterInfo[] parameters, Test test)
    {
        List<object> objects = new List<object>();

        for (int i = 0; i < parameters.Length; i++)
        {
            string[] parameterName = test.getFieldNameFromFilteredList(i).Split(' ');
            Type typeClassParamater = Type.GetType(parameterName[2]);
            FieldInfo fieldType = typeClassParamater.GetField(parameterName[0], BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            objects.Add(fieldType.GetValue(player.GetComponent(parameterName[2])));
        }

        return objects.ToArray();
    }
}
