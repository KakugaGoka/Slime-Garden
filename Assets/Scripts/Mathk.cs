using System.Collections.Generic;
using UnityEngine;
using System;

public static class Mathk
{
    public static float Clamp( float value, float min = 0, float max = 1 )
    {
        if (value > max) {
            return max;
        }
        else if (value < min) {
            return min;
        }
        else {
            return value;
        }
    }

    public static int Clamp( int value, int min = 0, int max = 1 )
    {
        if (value >= max) {
            return max;
        }
        else if (value <= min) {
            return min;
        }
        else {
            return value;
        }
    }

    public static float Lerp( float valueA, float valueB, float t )
    {
        float result = valueB - valueA;
        result *= t;
        result += valueA;
        return result;
    }

    public static float Max( float A, float B )
    {
        if (A > B) {
            return A;
        }
        else {
            return B;
        }
    }

    public static float Min( float A, float B )
    {
        if (A < B) {
            return A;
        }
        else {
            return B;
        }
    }

    public static float Max( float[] array )
    {
        float max = -100000000000000000000000000000000000000f;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] > max) {
                max = array[i];
            }
        }
        return max;
    }

    public static float Min( float[] array )
    {
        float min = 100000000000000000000000000000000000000f;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] < min) {
                min = array[i];
            }
        }
        return min;
    }

    public static float Max( List<float> list )
    {
        float max = -100000000000000000000000000000000000000f;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] > max) {
                max = list[i];
            }
        }
        return max;
    }

    public static float Min( List<float> list )
    {
        float min = 100000000000000000000000000000000000000f;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] < min) {
                min = list[i];
            }
        }
        return min;
    }

    public static float Mean( float[] array )
    {
        float total = 0f;
        for (int i = 0; i < array.Length; i++) {
            total += array[i];
        }
        return total / array.Length;
    }

    public static float AverageOfRange( float[] array, int n, int length )
    {
        float total = 0f;
        for (int i = 0; i < length; i++) {
            total += array[i + n];
        }
        return total / (float)length;
    }

    public static void NormalizeFloatArray( float[] array )
    {
        float max = Max( array );
        float min = Min( array );
        float range = max - min;
        if (!NearlyEqual( range, 0, Mathf.Epsilon )) {
            for (int i = 0; i < array.Length; i++) {
                array[i] -= min;
                array[i] /= range;
            }
        }
        else {
            for (int i = 0; i < array.Length; i++) {
                array[i] = 0;
            }
        }
    }

    public static List<float> NormalizeFloatList( List<float> list )
    {
        float max = Max( list );
        if (max != 0) {
            for (int i = 0; i < list.Count; i++) {
                list[i] /= max;
            }
        }
        return list;
    }

    public static void RescaleFloatArray( float[] array, float min, float max )
    {
        NormalizeFloatArray( array );

        BlendFloatArray( ( x, y ) => x * y, max - min, array );
        BlendFloatArray( ( x, y ) => x + y, min, array );
    }

    public static void RotateArray( float[] array, int amount, float[] output )
    {
        for (int i = 0; i < output.Length; i++) {
            output[i] = array[(i + amount) % array.Length];
        }
    }

    public static void BlendFloatArray( Func<float, float, float> blendMode, float value, float[] array )
    {
        for (int i = 0; i < array.Length; i++) {
            array[i] = blendMode( array[i], value );
        }
    }

    public static void BlendFloatArrays( Func<float, float, float> blendMode, float[] blendArray, float[] returnArray )
    {
        for (int i = 0; i < returnArray.Length; i++) {
            returnArray[i] = blendMode( returnArray[i], blendArray[i] );
        }
    }

    public static int RandomElement( int[] array )
    {
        int select = Mathf.FloorToInt( UnityEngine.Random.Range( 0, 10000000000 ) ) % (array.Length - 2);

        return array[select];
    }

    public static float RandomElement( float[] array )
    {
        int select = Mathf.FloorToInt( UnityEngine.Random.Range( 0, 10000000000 ) ) % (array.Length - 2);

        return array[select];
    }

    public static bool NearlyEqual( float floatA, float floatB, float range )
    {
        if (Mathf.Abs( floatA - floatB ) <= range) {
            return true;
        }
        else {
            return false;
        }
    }

    public static float SelectLesser( float floatA, float floatB, bool abs = false )
    {
        if (abs) {
            if (Mathf.Abs( floatA ) < Mathf.Abs( floatB )) {
                return floatA;
            }
            else {
                return floatB;
            }
        }
        else {
            if (floatA < floatB) {
                return floatA;
            }
            else {
                return floatB;
            }
        }
    }

    public static float SelectGreater( float floatA, float floatB, bool abs = false )
    {
        if (abs) {
            if (Mathf.Abs( floatA ) > Mathf.Abs( floatB )) {
                return floatA;
            }
            else {
                return floatB;
            }
        }
        else {
            if (floatA > floatB) {
                return floatA;
            }
            else {
                return floatB;
            }
        }
    }

    public static bool NearlyMod( float floatA, float mod, float range = 0 )
    {
        float modValue = Mathf.Abs( floatA % mod );
        float r = range;

        if (range == 0) {
            r = Mathf.Epsilon;
        }

        if (NearlyEqual( modValue, mod, r ) || NearlyEqual( modValue, 0, r )) {
            return true;
        }
        else {
            return false;
        }
    }

    public static float OneWayMod( float floatA, float mod )
    {
        float value = floatA;
        while (value < 0) {
            value += mod;
        }
        return value % mod;
    }

    public static int OneWayMod( int intA, int mod )
    {
        int value = intA;
        while (value < 0) {
            value += mod;
        }
        return value % mod;
    }

    public static float DistanceMod( float floatA, float mod )
    {
        float modValue = OneWayMod( floatA, mod );

        return SelectLesser( modValue, modValue - mod, true );
    }

    public static int RangeMod( int min, int max, int value )
    {
        int modRange = max - min;

        int modValue = value % modRange;

        modValue += min;

        return modValue;
    }

    //TODO: SplitRangeMod(List<Vector2>)
    //TODO: RangePack()

    public static bool InRange( float A, float B, float t, bool inclusiveMin = true, bool inclusiveMax = true )
    {
        Func<float, float, bool> compareMin;
        Func<float, float, bool> compareMax;

        if (inclusiveMin) {
            compareMin = ( x, y ) => { return x >= y; };
        }
        else {
            compareMin = ( x, y ) => { return x > y; };
        }

        if (inclusiveMax) {
            compareMax = ( x, y ) => { return x <= y; };
        }
        else {
            compareMax = ( x, y ) => { return x < y; };
        }

        if (compareMin( t, SelectLesser( A, B ) ) && compareMax( t, SelectGreater( A, B ) )) {
            return true;
        }
        else {
            return false;
        }
    }

    public static bool InRange( int A, int B, int t, bool inclusiveMin = true, bool inclusiveMax = true )
    {
        Func<float, float, bool> compareMin;
        Func<float, float, bool> compareMax;

        if (inclusiveMin) {
            compareMin = ( x, y ) => { return x >= y; };
        }
        else {
            compareMin = ( x, y ) => { return x > y; };
        }

        if (inclusiveMax) {
            compareMax = ( x, y ) => { return x <= y; };
        }
        else {
            compareMax = ( x, y ) => { return x < y; };
        }

        if (compareMin( t, SelectLesser( A, B ) ) && compareMax( t, SelectGreater( A, B ) )) {
            return true;
        }
        else {
            return false;
        }
    }

    public static int Quantize01( float input, int max )
    {
        return Mathf.RoundToInt( input * max );
    }

    public static float[] Clamp( float[] array )
    {
        float[] result = array;
        for (int i = 0; i < array.Length; i++) {
            result[i] = Clamp( array[i] );
        }
        return result;
    }

    public static float[] Clamp( float[] array, float min, float max )
    {
        float[] result = array;
        for (int i = 0; i < array.Length; i++) {
            result[i] = Clamp( array[i], min, max );
        }
        return result;
    }

    public static float[] AverageArrays( List<float[]> arrays )
    {
        float[] output = new float[arrays[0].Length];

        for (int n = 0; n < arrays[0].Length; n++)
            for (int i = 0; i < arrays.Count; i++) {
                float total = 0;
                {
                    total += arrays[i][n];
                }

                output[n] = total / (float)arrays.Count;
            }
        return output;
    }

    public static float[] AverageArrays( float[][] arrays )
    {
        int arraysL = arrays[0].Length;
        float[] output = new float[arraysL];

        for (int n = arraysL - 1; n > 0; n--) {
            float total = 0;
            for (int i = arrays.Length - 1; i > 0; i--) {
                total += arrays[i][n];
            }

            output[n] = total / (float)arrays.Length;
        }
        return output;
    }

    public static float[] DeltaArrays( List<float[]> arrays )
    {
        float[] output = new float[arrays[0].Length];

        for (int n = 0; n < arrays[0].Length; n++)
            for (int i = 0; i < arrays.Count - 1; i++) {
                float total = 0;
                {
                    total -= arrays[i][n] - arrays[i + 1][n];
                }

                //output[n] = total;
                output[n] = total / (arrays.Count - 1f);
            }
        return output;
    }

    public static float[] DeltaArrays( float[][] arrays )
    {
        float[] output = new float[arrays[0].Length];

        for (int n = 0; n < arrays[0].Length; n++)
            for (int i = 0; i < arrays.Length - 1; i++) {
                float total = 0;
                {
                    total -= arrays[i][n] - arrays[i + 1][n];
                }

                //output[n] = total;
                output[n] = total / (arrays.Length - 1f);
            }
        return output;
    }

    public static float[] PeakDetection( float[] array, int range, float noiseFloor )
    {
        float travel, rise;
        float[] result = new float[array.Length];

        for (int i = 0; i < array.Length - range; i++) {
            travel = 0;
            for (int n = 0; n < range; n++) {
                travel += array[i + n + 1] - array[i + n];
            }
            rise = array[i + range] - array[i] + noiseFloor;

            result[i] = travel / rise;
        }

        return result;
    }

    public static float PeakDetection( float[] array, int i, int range, float noiseFloor )
    {
        float travel, rise;
        float result;

        travel = 0;
        for (int n = 0; n < range; n++) {
            travel += array[i + n + 1] - array[i + n];
        }
        rise = array[i + range] - array[i] + noiseFloor;

        result = travel / rise;

        return result;
    }

    public static float[] CopyRange( float[] array, int amount, int index )
    {
        float[] output = new float[array.Length - amount];

        for (int i = 0; i < output.Length; i++) {
            output[i] = array[i + index];
        }

        return output;
    }

    public static float[] Shorten( float[] array, int amount )
    {
        return CopyRange( array, amount, array.Length - amount );
    }

    public static float ArrayTotal( float[] array, bool abs = false )
    {
        float total = 0;
        for (int i = 0; i < array.Length; i++) {
            if (abs) {
                total += Mathf.Abs( array[i] );
            }
            else {
                total += array[i];
            }
        }

        return total;
    }

    public static bool NearAny( float value, List<float> list, float range )
    {
        bool result = false;
        for (int i = 0; i < list.Count; i++) {
            if (NearlyEqual( list[i], value, range )) {
                result = true;
            }
        }
        return result;
    }

    public static float Sigmoid( float value, float compression, float xMid, float yMid )
    {
        float result;

        result = -compression * (value - xMid);
        result = 1 + Mathf.Pow( E, result );
        result = 1 / result;
        result -= 0.5f;
        result += yMid;

        return result;
    }

    public static float SigComp( float value, float compression, float lerp = 1 )
    {
        return Lerp( value, Sigmoid( value, compression, 0, 0 ), lerp );
    }

    public const float E = 2.718281828459045235f;
    public const float NoteBase = 1.059463094359295264561825294946341700779204317494185628559208431458761646063255722383768376863945569f;
    public const float FFTOffset = 0.0f;

    public static void ShiftArray( float[] array, int amount )
    {
        for (int i = array.Length - 1; i >= amount; i--) {
            array[i] = array[i - amount];
        }
    }

    public static void LinearEquation( Vector2 start, Vector2 end, out float slope, out float yOffset )
    {
        slope = (end.y - start.y) / (end.x - start.x);
        yOffset = end.y - (slope * end.x);
    }

    public static float XIntercept( float slope, float yOffset )
    {
        return -yOffset / slope;
    }

    public static void Derivative( float[] inFunction, float[] outDerivative )
    {
        float average;
        for (int i = 1; i < outDerivative.Length - 1; i++) {
            outDerivative[i] = inFunction[i + 1] - inFunction[i];
            average = inFunction[i] - inFunction[i - 1];
            average += outDerivative[i];
            outDerivative[i] = average / 2f;
        }
    }

    public static float FunctionOfX( float[] function, float x )
    {
        if (x <= function.Length - 1 && x >= 0) {
            Vector2 start = new Vector2( Mathf.FloorToInt( x ), function[Mathf.FloorToInt( x )] );
            Vector2 end = new Vector2( Mathf.CeilToInt( x ), function[Mathf.CeilToInt( x )] );

            LinearEquation( start, end, out float slope, out float offset );

            return slope * x + offset;
        }
        else {
            return 0;
        }
    }

    public static float RiemannSum( float[] function, float loBound, float hiBound, int resolution )
    {
        float result = 0;
        float x;
        for (int i = 0; i < resolution - 1; i++) {
            x = Mathf.Lerp( loBound, hiBound, i + 1 / (float)resolution );
            result += FunctionOfX( function, x ) / 2;
            x = Mathf.Lerp( loBound, hiBound, i / (float)resolution );
            result += FunctionOfX( function, x ) / 2;
        }
        result /= resolution;

        return result;
    }

    public static void Integral( float[] inFunction, float[] outIntegral )
    {
        float total = 0;
        for (int i = 0; i < outIntegral.Length; i++) {
            total += inFunction[i];
            outIntegral[i] = total;
        }
    }

    public static float FrequencyToPitch( float frequency )

    {
        return 12 * Mathf.Log( (frequency) / 440, 2 ) + 69;
    }

    public static int FrequencyToNote( float frequency )
    {
        return Mathf.RoundToInt( 12 * Mathf.Log( (frequency) / 440, 2 ) + 69 );
    }

    public static float PitchToFrequency( float note )
    {
        return Mathf.Pow( 2, (note - 69) / 12 ) * 440;
    }

    public static float Sigmoid( float x ) => 1f / (1f + (float)Math.Exp( -x ));

    public static float LogisticDensity( float x ) => (4 * Mathf.Pow( E, -x )) / Mathf.Pow( 1 + Mathf.Pow( E, -x ), 2 );

    public static float Similarity( float[] arrayA, float[] arrayB, float strictness )
    {
        float[] comparisons = new float[arrayA.Length];
        for (int i = 0; i < arrayA.Length; i++) {
            comparisons[i] = LogisticDensity( (arrayA[i] - arrayB[i]) * strictness );
        }
        return Min( comparisons );
    }
}