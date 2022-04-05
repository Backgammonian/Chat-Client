namespace InputBox
{
    public class InputBoxUtils
    {
        public bool AskNickname(string currentNickname, out string newNickname)
        {
            var inputBox = new InputBoxWindow("Change nickname", "Enter your new nickname", currentNickname);
            if (inputBox.ShowDialog() == true)
            {
                newNickname = inputBox.Answer;
                return true;
            }

            newNickname = string.Empty;
            return false;
        }
    }
}
