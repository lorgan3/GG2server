using GG2server.logic;
using GG2server.logic.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.data {
    public static class GaygonParser {
        private static Queue<string> Tokenize(string ggon) {
            Queue<string> tokens = new Queue<string>();
            int i = 1;
            int len = ggon.Length;

            while (i < len) {
                char chr = ggon[i];
                switch (chr) {
                    // basic punctuation: '{', '}', ':', '[', ']', and ','
                    case '{':
                    case '}':
                    case ':':
                    case '[':
                    case ']':
                    case ',':
                        tokens.Enqueue("" + chr);
                        i++;
                        continue;
                    // skip whitespace (space, tab, new line or carriage return)
                    case ' ':
                    case (char)10:
                    case (char)13:
                    case (char)9:
                        i++;
                        continue;
                }
                if (('a' <= chr && chr <= 'z') || ('A' <= chr && chr <= 'Z') || ('0' <= chr && chr <= '9') || chr == '_' || chr == '.' || chr == '+' || chr == '-') {
                    string identifier;
                    identifier = "";
                    while (('a' <= chr && chr <= 'z') || ('A' <= chr && chr <= 'Z') || ('0' <= chr && chr <= '9') || chr == '_' || chr == '.' || chr == '+' || chr == '-') {
                        if (i > len) {
                            throw new GGonException("Error when tokenising GGON: unexpected end of text while parsing string");
                        }
                        identifier += chr;
                        i++;
                        chr = ggon[i];
                    }
                    tokens.Enqueue("%" + identifier);
                    continue;
                }

                // string
                if (chr == '\'') {
                    string str;
                    str = "";
                    i++;
                    chr = ggon[i];
                    while (chr != '\'') {
                        if (i > len) {
                            throw new GGonException("Error when tokenising GGON: unexpected end of text while parsing string");
                        }
                        // escaping
                        i++;
                        if (chr == '\\') {
                            i++;
                            chr = ggon[i];
                            if (chr == '\'' || chr == '\\')
                                str += chr;
                            // new line escape
                            else if (chr == 'n')
                                str += (char)10;
                            // carriage return escape
                            else if (chr == 'r')
                                str += (char)13;
                            // tab escape
                            else if (chr == 't')
                                str += (char)9;
                            // null byte escape
                            else if (chr == '0')
                                str += (char)0;
                            else {
                                throw new GGonException("Error when tokenising GGON: unknown escape sequence \\" + chr + '"');
                            }
                        } else {
                            str += chr;
                        }
                        i++;
                        chr = ggon[i];
                    }
                    if (chr != '\'') {
                        throw new GGonException("Error when tokenising GGON: unexpected character \"" + chr + " while parsing string, expected " + '\'' + "\"");
                    }
                    i++;
                    chr = ggon[i];

                    tokens.Enqueue('%' + str);
                    continue;
                }

                throw new GGonException("Error when tokenising GGON: unexpected character \"" + chr + '"');
            }
            return tokens;
        }

        private static object Parse(Queue<string> tokens) {
            while (tokens.Count != 0) {
                string token = tokens.Dequeue();

                // String
                if (token[0] == '%') {
                    return token.Substring(1);
                }

                // GGON has only three primitives - it could only be string, opening { or opening [
                if (token != "{" && token != "[") {
                    throw new GGonException("Error when parsing GGON: unexpected token \"" + token + '"');
                }

                var map = new Dictionary<string, object>();

                if (token == "{") { //GGON map
                    token = tokens.Peek();
                    if (token == "}") { // Empty map
                        tokens.Dequeue();
                        return map;

                    } else if (token[0] == '%') { // String
                        while (tokens.Count != 0) {
                            string key = tokens.Dequeue().Substring(1);
                            token = tokens.Dequeue();
                            // Following token must be a : as we have a key
                            if (token != ":") {
                                throw new GGonException("Error when parsing GGON: unexpected token \"" + token + "\" after key");
                            }

                            map.Add(key, Parse(tokens));
                            token = tokens.Dequeue();

                            // After key, colon and value, next token must be , or }
                            if (token == ",")
                                continue;
                            else if (token == "}")
                                return map;
                            else
                                throw new GGonException("Error when parsing GGON: unexpected token \"" + token + "\" after value");
                        }
                    } else {
                        throw new GGonException("Error when parsing GGON: unexpected token \"" + token + '"');
                    }

                } else { // GGON list
                    token = tokens.Peek();
                    if (token == "]") { // Empty list
                        tokens.Dequeue();
                        map.Add("Length", 0);
                        return map;

                    } else {
                        var len = 0;
                        // Parse each value in our list
                        while (tokens.Count != 0) {
                            // Now we recurse to parse our value!
                            map.Add("" + len, Parse(tokens));
                            len++;

                            token = tokens.Dequeue();

                            // After value, next token must be , or ]
                            if (token == ",")
                                continue;
                            else if (token == "]") {
                                map.Add("Length", len);
                                return map;
                            } else
                                throw new GGonException("Error when parsing GGON: unexpected token \"" + token + "\" after value");
                        }
                    }
                }
            }

            return null;
        }

        public static Dictionary<string, object> Decode(string ggon) {
            Queue<string> tokens = Tokenize(ggon);

            try {
                return (Dictionary<string, object>)Parse(tokens);
            } catch (InvalidCastException ce) {
                throw new GGonException("Supplied ggon was not a valid map or list.");
            }
        }
    }
}
