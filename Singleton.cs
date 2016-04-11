using UnityEngine;

namespace ifup
{
    /// <summary>
    /// source: http://wiki.unity3d.com/index.php?title=Singleton#Generic_Based_Singleton_for_MonoBehaviours
    /// 
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T mInstance;

        private static object mLock = new object();
        private static bool mDynamic = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting) {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed on application quit." +
                                     " Won't create again - returning null.");
                    return null;
                }

                lock (mLock) {
                    if (mInstance == null) {
                        mInstance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1) {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                           " - there should never be more than 1 singleton!" +
                                           " Reopenning the scene might fix it.");
                            return null;
                        }

                        if (mInstance == null) {
                            GameObject singleton = new GameObject();
                            mInstance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);

                            mDynamic = true;

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                      " is needed in the scene, so '" + singleton +
                                      "' was created with DontDestroyOnLoad.");
                        }
                    }

                    return mInstance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        void OnDestroy()
        {
            if (mDynamic) {
                applicationIsQuitting = true;
                mDynamic = false;
            }
        }
    }
}