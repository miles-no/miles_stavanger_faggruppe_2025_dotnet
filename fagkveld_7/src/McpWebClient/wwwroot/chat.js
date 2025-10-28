/**
 * MCP Chat Interface - Client-side JavaScript
 * Handles the chat UI and communication with the backend API
 */

// State management
let conversationHistory = [];
let isProcessing = false;

/**
 * Adds a message to the chat UI
 * @param {string} role - The message role: 'user', 'assistant', or 'error'
 * @param {string} content - The message content to display
 */
function addMessage(role, content) {
    const chatContainer = document.getElementById('chatContainer');

    // Remove welcome message if it exists (first message only)
    const welcomeMessage = chatContainer.querySelector('.welcome-message');
    if (welcomeMessage) {
        welcomeMessage.remove();
    }

    // Create message element
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${role}`;

    // Determine the display label for the role
    const roleLabel = role === 'user' ? 'You' : role === 'assistant' ? 'Assistant' : 'Error';

    messageDiv.innerHTML = `
        <div>
            <div class="message-role">${roleLabel}</div>
            <div class="message-content">${content}</div>
        </div>
    `;

    chatContainer.appendChild(messageDiv);
    chatContainer.scrollTop = chatContainer.scrollHeight;
}

/**
 * Shows a loading indicator while waiting for the assistant's response
 */
function addLoadingMessage() {
    const chatContainer = document.getElementById('chatContainer');
    const loadingDiv = document.createElement('div');
    loadingDiv.className = 'message assistant';
    loadingDiv.id = 'loadingMessage';
    loadingDiv.innerHTML = `
        <div>
            <div class="message-role">Assistant</div>
            <div class="message-content">
                <span class="loading"></span>
                <span class="loading" style="animation-delay: 0.2s;"></span>
                <span class="loading" style="animation-delay: 0.4s;"></span>
            </div>
        </div>
    `;
    chatContainer.appendChild(loadingDiv);
    chatContainer.scrollTop = chatContainer.scrollHeight;
}

/**
 * Removes the loading indicator
 */
function removeLoadingMessage() {
    const loadingMessage = document.getElementById('loadingMessage');
    if (loadingMessage) {
        loadingMessage.remove();
    }
}

/**
 * Sends a message to the chat API and handles the response
 */
async function sendMessage() {
    // Prevent sending multiple messages at once
    if (isProcessing) return;

    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById('sendButton');
    const message = messageInput.value.trim();

    // Don't send empty messages
    if (!message) return;

    // Update UI state
    isProcessing = true;
    sendButton.disabled = true;
    messageInput.value = '';
    messageInput.style.height = 'auto';

    // Add user message to UI
    addMessage('user', message);

    // Add to conversation history for context
    conversationHistory.push({ role: 'user', content: message });

    // Show loading indicator
    addLoadingMessage();

    try {
        // Get optional API key from input
        const apiKey = document.getElementById('apiKeyInput').value.trim();

        // Send request to backend
        const response = await fetch('/api/chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                messages: conversationHistory,
                apiKey: apiKey || null
            }),
        });

        removeLoadingMessage();

        // Handle errors
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.detail || 'Failed to get response');
        }

        // Get the response data
        const data = await response.json();

        // Add assistant message to UI
        addMessage('assistant', data.message);

        // Add to conversation history
        conversationHistory.push({ role: 'assistant', content: data.message });

    } catch (error) {
        removeLoadingMessage();
        addMessage('error', `Error: ${error.message}`);
    } finally {
        // Re-enable input
        isProcessing = false;
        sendButton.disabled = false;
        messageInput.focus();
    }
}

/**
 * Sends one of the example prompts
 * @param {string} text - The example prompt text
 */
function sendExample(text) {
    const messageInput = document.getElementById('messageInput');
    messageInput.value = text;
    sendMessage();
}

/**
 * Handles Enter key press in the message input
 * Enter = send, Shift+Enter = new line
 * @param {KeyboardEvent} event
 */
function handleKeyPress(event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        sendMessage();
    }
}

/**
 * Auto-resize the textarea as the user types
 */
function setupTextareaAutoResize() {
    const messageInput = document.getElementById('messageInput');
    messageInput.addEventListener('input', function() {
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 200) + 'px';
    });
}

/**
 * Initialize the app when the page loads
 */
function initializeApp() {
    setupTextareaAutoResize();

    // Focus on input for better UX
    document.getElementById('messageInput').focus();
}

// Initialize when the page loads
window.addEventListener('load', initializeApp);
