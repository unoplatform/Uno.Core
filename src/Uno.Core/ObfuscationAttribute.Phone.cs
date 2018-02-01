// ******************************************************************
// Copyright � 2015-2018 nventive inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// ******************************************************************
using System;
using System.Net;

namespace Uno
{
    public sealed class ObfuscationAttribute : Attribute
    {
        private bool m_applyToMembers = false;
        private bool m_exclude = true;
        private string m_feature = "all";
        private bool m_strip = true;

        public bool ApplyToMembers
        {
            get
            {
                return this.m_applyToMembers;
            }
            set
            {
                this.m_applyToMembers = value;
            }
        }

        public bool Exclude
        {
            get
            {
                return this.m_exclude;
            }
            set
            {
                this.m_exclude = value;
            }
        }

        public string Feature
        {
            get
            {
                return this.m_feature;
            }
            set
            {
                this.m_feature = value;
            }
        }

        public bool StripAfterObfuscation
        {
            get
            {
                return this.m_strip;
            }
            set
            {
                this.m_strip = value;
            }
        }
    }
}
