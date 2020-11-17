using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// Represents the status of a test
// 1. wait = wait for conditions to be met for startup
// 2. in run = test started and not finished yet
// 3. failed = test failed
// 4. passed = test successful
public enum TestState { WAIT, RUN, FAILED, PASSED };

// Represents the operator to apply to a game coordinate for a triggerCondition position
// 1. X >= VAL
// 2. X == VAL
// 3. X <= VAL
// 4. X NOT MONITORED
public enum Operator { GREATER_EQUAL, EQUAL, LESS_EQUAL, DONT_CHECK};


public class Test
{
    MethodInfo methodInfo; // information about the test method
    string scriptName; // name of the script that contains this method
    TestState testState; // state of the test during its life cycle

    List<TriggerCondition> triggerConditions; // contains a list of trigger conditions
    List<string[]> filteredLists; // contains lists for filtered parametes
    int[] indexFieldsForEachParameters; // contains indexes for parameter matching

    bool isSelected; // if the test was triggered by the tester
    bool isParameterTabActive; // state of parameter tab
    bool isTriggerConditionsActive; // state of trigger conditions tab
    bool isConstraintTabActive; // state of constraint tab

    bool triggerPosition, triggerLevel; // state of triggerConditions position and level.
    Operator[] operatorValues; // Ex. for x,y,z = [ <=, ==, >= ].
    int[] indexOperators; // Ex. for x,y,z = [ 0, 2, 1 ] == [>=, <=, =]
    string posX, posY, posZ; // for triggerCondition position.
    int level; // for triggerCondition level.

    float refreshTime; // represents the refresh time of a test
    float nextRefreshTime; //next value for re-freshing
    int limitFrame; // closing constraint of a unit test
    int actualFrame; // actual frame after run test
    int numberOfRipetitions; // represents the number of times a test must repeat
    int actualRipetitions; // represents the i-th time that the test has been repeated

    bool stopRunningTest; // When a test ends its repetitions this variable changes to true (The last test state set remains)
    bool badSetting; // Switch to true when a test has had a bad configuration
    string errorMessage; // Contains a message when an exception is raised following the test run

    public Test(MethodInfo info, string nomeScript, List<TriggerCondition> triggerConditions)
    {
        this.methodInfo = info;
        this.scriptName = nomeScript;
        testState = TestState.WAIT;

        this.triggerConditions = triggerConditions;
        filteredLists = new List<string[]>();
        indexFieldsForEachParameters = new int[info.GetParameters().Length];

        isSelected = false;
        isTriggerConditionsActive = false;
        isConstraintTabActive = false;
        isParameterTabActive = false;

        triggerPosition = false;
        triggerLevel = false;
        operatorValues = new Operator[3] { Operator.GREATER_EQUAL, Operator.GREATER_EQUAL, Operator.GREATER_EQUAL };
        indexOperators = new int[3] { 0, 0, 0 };
        level = 0;
        posX = "0";
        posY = "0";
        posZ = "0";

        refreshTime = 0.1f;
        nextRefreshTime = 0;
        limitFrame = 0;
        actualFrame = 0;
        numberOfRipetitions = 0;
        actualRipetitions = 0;

        stopRunningTest = false;
        badSetting = false;
        errorMessage = "none";
    }

    
    // ------------------------------------ FILTERED LIST & TRIGGER-CONDITION LIST MANAGEMENT  ------------------------------------- //
    public List<string[]> getFilteredLists() { return filteredLists; }  
    public string[] getFilteredList(int parameterIndex) { return filteredLists[parameterIndex]; }
    public string getFieldNameFromFilteredList(int parameterIndex) { return filteredLists[parameterIndex][indexFieldsForEachParameters[parameterIndex]]; }
    public void setFilteredList(int parameterIndex, string[] filteredFields)
    {
        if (parameterIndex + 1 > filteredLists.Count)
            filteredLists.Add(filteredFields);
    }
    

    public int[] getIndexFieldForEachParameters() { return indexFieldsForEachParameters; }
    public int getIndexForParameter(int index) { return indexFieldsForEachParameters[index]; }
    public bool setIndexForParameter(int parameterIndex, int value)
    {
        if (indexFieldsForEachParameters[parameterIndex] != value)
        {
            indexFieldsForEachParameters[parameterIndex] = value;
            return true;
        }

        return false;
    }

    public List<TriggerCondition> getTriggerConditions() { return triggerConditions; }
    public void setTriggerConditions(List<TriggerCondition> triggerConditions) { this.triggerConditions = triggerConditions; }



    // ------------------------------------------ GENERAL INFO MANAGEMENT  ------------------------------------------ //
    public MethodInfo getMethodInfo() { return methodInfo; }
    public void setMethodInfo(MethodInfo info) { this.methodInfo = info; }
    public string getScriptName() { return scriptName; }
    public string getFullName() { return scriptName+ ": " + methodInfo.ToString(); }

    public TestState getTestState() { return testState; }
    public bool setTestState(TestState newState) 
    { 
        if(testState != newState)
        {
            testState = newState;
            return true;
        }

        return false;
    }



    // ------------------------------------------ SELECTION AND TAB MANAGEMENT  ------------------------------------------ //
    public bool getSelected() { return isSelected; }
    public bool setSelected(bool state) 
    { 
        if(this.isSelected != state)
        {
            this.isSelected = state;
            return true;
        }

        return false;
    }

    public bool getParametersTabActive() { return isParameterTabActive; }
    public bool getTriggerConditionsActive() { return isTriggerConditionsActive; }
    public bool getConstraintsActive() { return isConstraintTabActive; }
    public void setParametersTabActive(bool tab) { isParameterTabActive = tab; }
    public void setTriggerTabActive(bool tab) { isTriggerConditionsActive = tab; }
    public void setConstraintsActive(bool tab) { isConstraintTabActive = tab; }



    // ------------------------------------------ TAB VALUES MANAGEMENT  ------------------------------------------ //
    public bool getTriggerPosition() { return triggerPosition; }
    public bool setTriggerPosition(bool state) 
    { 
        if(triggerPosition != state)
        {
            triggerPosition = state;
            return true;
        }

        return false;
    }
    public bool getTriggerLevel() { return triggerLevel; }
    public bool setTriggerLevel(bool state)
    {
        if (triggerLevel != state)
        {
            triggerLevel = state;
            return true;
        }
        return false;
    }
   
    public Operator[] getOperators() { return operatorValues; }
    public Operator getOperator(int index) { return operatorValues[index]; }

    public int getIndexOperator(int index) { return indexOperators[index]; }
    public bool setIndexOperator(int index, int value)
    {
        if(indexOperators[index] != value)
        {
            indexOperators[index] = value;
            setOperator(index, value);
            return true;
        }

        return false;
    }
    public void setOperator(int index, int value)
    {
        switch(value)
        {
            case 0: operatorValues[index] = Operator.GREATER_EQUAL;
                                            break;
            case 1: operatorValues[index] = Operator.EQUAL;
                                            break;
            case 2: operatorValues[index] = Operator.LESS_EQUAL;
                                            break;
            case 3: operatorValues[index] = Operator.DONT_CHECK;
                                            break;
        }
    }

    public string getX() { return posX; }
    public string getY() { return posY; }
    public string getZ() { return posZ; }
    public bool setX(string x) 
    {
        if (posX != x)
        {
            posX = x;
            return true;
        }
        return false;
    }
    public bool setY(string y)
    {
        if (posY != y)
        {
            posY = y;
            return true;
        }
        return false;
    }
    public bool setZ(string z)
    {
        if (posZ != z)
        {
            posZ = z;
            return true;
        }
        return false;
    }
    
    public int getLevel() { return level; }
    public bool setLevel(int newLevel) 
    { 
        if(level != newLevel)
        {
            level = newLevel;
            return true;
        }

        return false;
    }



    // ------------------------------------------ CONSTRAINTS VALUES MANAGEMENT  ------------------------------------------ //
    public int getFrameLimit() { return limitFrame; }
    public int getActualFrame() { return actualFrame; }
    public float getRefreshTime() { return refreshTime; }
    public float getNextRefreshTime() { return nextRefreshTime; }
    public int getNumberOfRipetitions() { return numberOfRipetitions; }
    public int getActualRipetition() { return actualRipetitions; }

    public bool setLimitFrame(int limit)
    {
        if (limitFrame != limit)
        {
            limitFrame = limit;
            return true;
        }

        return false;
    }
    public bool seteRefreshTime(float time)
    {
        if (refreshTime != time)
        {
            refreshTime = time;
            return true;
        }

        return false;
    }
    public bool setNumberOfRipetitions(int ripetitions)
    {
        if (numberOfRipetitions != ripetitions)
        {
            numberOfRipetitions = ripetitions;
            return true;
        }

        return false;
    }
   
    public bool incrementActualFrame()
    {
        if (actualFrame < limitFrame)
        {
            actualFrame += 1;
            return true;
        }

        return false;
    }
    public void incrementRipetitions()
    {
        if (numberOfRipetitions > 0)
        {
            if (actualRipetitions >= numberOfRipetitions) stopRunningTest = true;
            else actualRipetitions += 1;
        }
        else actualRipetitions += 1;

    }
    public void updateNextRefreshTime(float value) { nextRefreshTime = value; }



    // ------------------------------------------ WARNING AND STOP MANAGEMENT  ------------------------------------------ //
    public bool getStopRunningTest() { return stopRunningTest; }
    public bool getBadSetting() { return badSetting; }
    public void setBadSetting(bool setting) { badSetting = setting; }
    public string getErrorMessage() { return errorMessage; }
    public void setErrorMessage(string s) { errorMessage = s; }
}



/*
 * A trigger condition is primarily a Boolean function.
 * Instances of this class contain information about the triggerConditions created by the tester.
 */

public class TriggerCondition
{
    MethodInfo info; // boolean function info
    bool isSelected; // state of selection

    public TriggerCondition(MethodInfo info)
    {
        this.info = info;
        isSelected = false;
    }

    public MethodInfo getMethodInfo() { return info; }
    public bool getSelected() { return isSelected; }
    public bool setSelected(bool selected)
    {
        if(isSelected != selected)
        {
            isSelected = selected;
            return true;
        }

        return false;
    }
}